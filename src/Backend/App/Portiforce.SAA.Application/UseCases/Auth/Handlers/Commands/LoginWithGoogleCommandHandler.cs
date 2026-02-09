using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence;
using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence.Client;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Services.Tenant;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Auth.Projections;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Auth;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Handlers.Commands;

public sealed class LoginWithGoogleCommandHandler(
	IGoogleAuthProvider googleProvider,
	ITokenGenerator tokenGenerator,
	ITenantLimitsService tenantLimitsService,
	IAccountReadRepository accountReadRepository,
	ITenantReadRepository tenantReadRepository,
	IExternalIdentityReadRepository externalIdentityReadRepository,
	IExternalIdentityWriteRepository externalIdentityWriteRepository,
	IUnitOfWork unitOfWork) 
	: IRequestHandler<LoginWithGoogleCommand, AuthResponse>
{
	public async ValueTask<AuthResponse> Handle(LoginWithGoogleCommand request, CancellationToken ct)
	{
		// 1. Verify Token with Google
		GoogleUserInfo googleUser = await googleProvider.VerifyAsync(request.IdToken, ct);

		// 2. Find Account by Google Link
		ExternalIdentityDetails? identity = await externalIdentityReadRepository.FindGoogleIdentityAsync(googleUser.ExternalId, ct);

		string token;
		IAccountInfo accountInfo;

		if (!Email.TryCreate(googleUser.Email, out Email userEmailModel))
		{
			// This is technically a "Bad Request" or "Validation Error".
			throw new DomainValidationException($"Invalid email received from provider: {googleUser.Email}");
		}

		if (identity is not null)
		{
			// Case A: User is already linked
			TenantId resolvedTenantId = identity.TenantId;

			// if Request specified a Tenant (e.g. subdomain), ensure it matches
			if (request.TenantId is { IsEmpty: false } t && t != resolvedTenantId)
			{
				throw new DomainValidationException($"User belongs to tenant {resolvedTenantId}, not {request.TenantId}");
			}

			// User has logged in before
			AccountDetails? accountDetails = await accountReadRepository.GetByIdAsync(identity.AccountId, ct);
			if (accountDetails == null)
			{
				throw new NotFoundException("AccountDetails", googleUser.Email);
			}
			EnsureCanLoginByAccountState(accountDetails.State);

			// check tenant availability
			TenantSummary? tenantInfo = await tenantReadRepository.GetSummaryByIdAsync(resolvedTenantId, ct);
			if (tenantInfo == null)
			{
				throw new NotFoundException("Tenant", resolvedTenantId);
			}
			EnsureCanLoginByTenantState(tenantInfo.State);

			accountInfo = accountDetails;
		}
		else
		{
			// Case B: First time login (Auto-Link)
			if (request.TenantId is { IsEmpty: false } tenantId)
			{
				AccountDetails? accountDetails = await accountReadRepository.GetByEmailAndTenantAsync(userEmailModel, tenantId, ct);
				if (accountDetails is null)
				{
					throw new NotFoundException("Account", googleUser.Email);
				}

				EnsureCanLoginByAccountState(accountDetails.State);

				TenantSummary tenantSummary = await tenantReadRepository.GetSummaryByIdAsync(accountDetails.TenantId, ct)
				                              ?? throw new NotFoundException("Tenant", accountDetails.TenantId);

				EnsureCanLoginByTenantState(tenantSummary.State);

				await LinkGoogleIdentityAsync(
					accountDetails.Id,
					accountDetails.TenantId,
					googleUser.ExternalId,
					ct);
				accountInfo = accountDetails;
			}
			else
			{
				List<AccountListItem> accounts = await accountReadRepository.GetByEmailAsync(userEmailModel, ct);

				if (accounts.Count == 0)
				{
					throw new NotFoundException("Account", googleUser.Email);
				}

				if (accounts.Count > 1)
				{
					throw new DomainValidationException("Multiple tenants use this email. Specify X-Tenant-ID.");
				}

				var accountListItem = accounts[0];
				EnsureCanLoginByAccountState(accountListItem.State);

				TenantSummary tenantSummary = await tenantReadRepository.GetSummaryByIdAsync(accountListItem.TenantId, ct)
										   ?? throw new NotFoundException("Tenant", accountListItem.TenantId);

				EnsureCanLoginByTenantState(tenantSummary.State);

				if (accountListItem.State == AccountState.PendingActivation)
				{
					await tenantLimitsService.EnsureTenantCanInviteOrActivateUserAsync(tenantSummary, ct);

					// todo: handle change via domain model?
					// targetAccount.State = AccountState.Active; 
					// await accountWriteRepository.UpdateAsync(targetAccount, ct);
				}

				await LinkGoogleIdentityAsync(
					accountListItem.Id,
					accountListItem.TenantId,
					googleUser.ExternalId,
					ct);
				accountInfo = accountListItem;
			}
		}

		// 4. Generate JWT
		token = tokenGenerator.GenerateAccessToken(accountInfo);

		return new AuthResponse(token, "dummy-refresh-token", DateTime.Now.AddDays(7));
	}

	private async Task LinkGoogleIdentityAsync(AccountId accountId, TenantId tenantId, string googleUserExternalId, CancellationToken ct)
	{
		ExternalIdentity newIdentity = ExternalIdentity.Create(
			accountId,
			tenantId,
			AuthProvider.Google,
			googleUserExternalId);

		try
		{
			await externalIdentityWriteRepository.AddAsync(newIdentity, ct);
			await unitOfWork.SaveChangesAsync(ct);
		}
		catch (UniqueConstraintViolationException)
		{
			throw new ConflictException("Google identity already linked.");
		}
	}
	
	private void EnsureCanLoginByAccountState(AccountState accountState)
	{
		// use only cases and states when person is able to log in
		bool canLogin = accountState is AccountState.Active or AccountState.PendingActivation;

		if (!canLogin)
		{
			throw new DomainValidationException($"User state is not valid for login flow {accountState}");
		}
	}

	private void EnsureCanLoginByTenantState(TenantState tenantState)
	{
		// use only cases and states when tenant is able to accept user logins
		bool canLogin = tenantState is TenantState.Active;

		if (!canLogin)
		{
			throw new DomainValidationException($"Tenant state is not valid for login flow {tenantState}");
		}
	}
}
