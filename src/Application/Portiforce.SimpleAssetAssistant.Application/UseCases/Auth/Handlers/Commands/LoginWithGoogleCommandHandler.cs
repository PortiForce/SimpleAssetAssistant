using Portiforce.SimpleAssetAssistant.Application.Entitlements;
using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Auth.Models;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Client;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Resolvers;
using Portiforce.SimpleAssetAssistant.Application.Models.Auth;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;
using Portiforce.SimpleAssetAssistant.Core.Exceptions;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Handlers.Commands;

public sealed class LoginWithGoogleCommandHandler(
	IGoogleAuthProvider googleProvider,
	ITokenGenerator tokenGenerator,
	ITenantEntitlementsResolver tenantEntitlementsResolver,
	IAccountReadRepository accountReadRepository,
	ITenantReadRepository tenantReadRepository,
	IExternalIdentityReadRepository externalIdentityReadRepository,
	IExternalIdentityWriteRepository externalIdentityWriteRepository
	) : IRequestHandler<LoginWithGoogleCommand, AuthResponse>
{
	public async ValueTask<AuthResponse> Handle(LoginWithGoogleCommand request, CancellationToken ct)
	{
		// 1. Verify Token with Google
		GoogleUserInfo googleUser = await googleProvider.VerifyAsync(request.IdToken, ct);

		// 2. Find Account by Google Link
		ExternalIdentityDetails? identity = await externalIdentityReadRepository.FindGoogleIdentityAsync(googleUser.ExternalId, ct);

		string token;
		IAccountInfo accountInfo;
		
		if (identity != null)
		{
			// Case A: User is already linked
			TenantId resolvedTenantId = identity.TenantId;

			// if Request specified a Tenant (e.g. subdomain), ensure it matches
			if (request.TenantId is { IsEmpty: false } && request.TenantId != resolvedTenantId)
			{
				throw new DomainValidationException($"User belongs to tenant {resolvedTenantId}, not {request.TenantId}");
			}

			// User has logged in before
			AccountDetails?  accountDetails = await accountReadRepository.GetByIdAsync(identity.AccountId, ct);
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

			// We MUST rely on Email lookup.
			// If Request.TenantId is null, we can't search (unless email is unique globally).
			// Assuming Email is unique PER TENANT, Global Login is tricky here without a TenantId.

			// For MVP: Let's assume Global Login requires finding ANY account with that email
			// Or enforce TenantId for new users. 

			// Let's go with: Try to find by Email.
			List<AccountListItem> accounts = await accountReadRepository.GetByEmailAsync(googleUser.Email, ct);

			if (!accounts.Any())
			{
				throw new NotFoundException("Account", googleUser.Email);
			}

			// todo tech
			// If multiple tenants have this email, we might need to ask user to pick.
			// For MVP, pick the first one or matches request.TenantId
			AccountListItem? targetAccount = request.TenantId != null
				? accounts.FirstOrDefault(a => a.TenantId == request.TenantId)
				: accounts.FirstOrDefault();

			if (targetAccount == null)
			{
				throw new NotFoundException("Account", googleUser.Email);
			}

			EnsureCanLoginByAccountState(targetAccount.State);
			
			// check tenant availability
			var resolvedTenantId = targetAccount.TenantId;
			TenantSummary? tenantInfo = await tenantReadRepository.GetSummaryByIdAsync(resolvedTenantId, ct);
			if (tenantInfo == null)
			{
				throw new NotFoundException("Tenant", resolvedTenantId);
			}
			EnsureCanLoginByTenantState(tenantInfo.State);

			if (targetAccount.State == AccountState.NotVerified)
			{
				// Pass the TenantID and the limit from the Plan
				await EnsureTenantCanActivateUserAsync(tenantInfo, ct);

				// todo: handle change via domain model?
				// targetAccount.State = AccountState.Active; 
				// await accountWriteRepository.UpdateAsync(targetAccount, ct);
			}

			// Create Link
			ExternalIdentity newIdentity = ExternalIdentity.Create(
				targetAccount.Id,
				resolvedTenantId,
				AuthProvider.Google,
				googleUser.ExternalId); 

			await externalIdentityWriteRepository.AddAsync(newIdentity, ct);

			accountInfo = targetAccount;
		}

		// 4. Generate JWT
		token = tokenGenerator.Generate(accountInfo);

		return new AuthResponse(token, "dummy-refresh-token", 3600);
	}

	private async Task EnsureTenantCanActivateUserAsync(TenantSummary tenant, CancellationToken ct)
	{
		TenantEntitlements tenantEntitlements = tenantEntitlementsResolver.Resolve(tenant.Plan);

		int maxUsers = tenantEntitlements.MaxUsers;

		int currentActiveUsers = await accountReadRepository.GetActiveUserCountAsync(tenant.Id, ct);

		if (currentActiveUsers >= maxUsers)
		{
			throw new DomainValidationException(
				$"Tenant '{tenant.Name}' has reached the maximum of {maxUsers} active users. Please contact your admin to upgrade.");
		}
	}

	private void EnsureCanLoginByAccountState(AccountState accountState)
	{
		// use only cases and states when person is able to log in
		bool canLogin = accountState is AccountState.Active or AccountState.NotVerified;

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
