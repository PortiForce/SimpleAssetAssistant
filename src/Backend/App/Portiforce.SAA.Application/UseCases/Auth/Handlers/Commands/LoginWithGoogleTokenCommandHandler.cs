using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Models.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence.Client;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Services.Auth;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Auth.Projections;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Handlers.Commands;

public sealed class LoginWithGoogleTokenCommandHandler(
	IGoogleAuthProvider googleProvider,
	ITokenGenerator tokenGenerator,
	ILoginValidationService loginValidationService,
	IAccountReadRepository accountReadRepository,
	ITenantReadRepository tenantReadRepository,
	IExternalIdentityReadRepository externalIdentityReadRepository)
	: IRequestHandler<LoginWithGoogleTokenCommand, TypedResult<AuthResponse>>
{
	public async ValueTask<TypedResult<AuthResponse>> Handle(LoginWithGoogleTokenCommand request, CancellationToken ct)
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

		if (identity is null)
		{
			//(Auto-Link is not possible here as person has to accept invite first)
			return TypedResult<AuthResponse>.Fail(ResultError.NotSupported("person has to be linked via invite flow"));
		}

		// Case A: User is already linked
		TenantId resolvedTenantId = identity.TenantId;

		// if Request specified a Tenant (e.g. subdomain), ensure it matches
		if (request.TenantId is { IsEmpty: false } t && t != resolvedTenantId)
		{
			throw new DomainValidationException(
				$"User belongs to tenant {resolvedTenantId}, not {request.TenantId}");
		}

		// User has logged in before
		AccountDetails? accountDetails = await accountReadRepository.GetByIdAsync(identity.AccountId, ct);
		if (accountDetails == null)
		{
			return TypedResult<AuthResponse>.Fail(ResultError.NotFound("Account", identity.AccountId));
		}

		var accountValidationResult = loginValidationService.EnsureCanLoginByAccountState(accountDetails.State);
		if (!accountValidationResult.IsSuccess)
		{
			return TypedResult<AuthResponse>.Fail(ResultError.Validation(accountValidationResult.Error.Message));
		}

		// check tenant availability
		TenantSummary? tenantInfo = await tenantReadRepository.GetSummaryByIdAsync(resolvedTenantId, ct);
		if (tenantInfo == null)
		{
			return TypedResult<AuthResponse>.Fail(ResultError.NotFound("Tenant", resolvedTenantId));
		}

		var tenantValidationResult = loginValidationService.EnsureCanLoginByTenantState(tenantInfo.State);
		if (!tenantValidationResult.IsSuccess)
		{
			return TypedResult<AuthResponse>.Fail(ResultError.Validation(tenantValidationResult.Error.Message));
		}

		accountInfo = accountDetails;

		token = tokenGenerator.GenerateAccessToken(accountInfo);

		return TypedResult<AuthResponse>.Ok(new AuthResponse(token, "dummy-refresh-token", DateTime.Now.AddDays(7)));
	}
}
