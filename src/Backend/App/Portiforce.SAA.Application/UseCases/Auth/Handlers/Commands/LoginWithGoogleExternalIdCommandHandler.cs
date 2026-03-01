using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Persistence.Auth;
using Portiforce.SAA.Application.Interfaces.Persistence.Client;
using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.Interfaces.Services.Auth;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Auth.Projections;
using Portiforce.SAA.Application.UseCases.Auth.Result;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Auth.Handlers.Commands;

public sealed class LoginWithGoogleExternalIdCommandHandler(
	ILoginValidationService loginValidationService,
	IAccountReadRepository accountReadRepository,
	ITenantReadRepository tenantReadRepository,
	IExternalIdentityReadRepository externalIdentityReadRepository)
	: IRequestHandler<LoginWithGoogleExternalIdCommand, TypedResult<LoginWithGoogleResult>>
{
	public async ValueTask<TypedResult<LoginWithGoogleResult>> Handle(LoginWithGoogleExternalIdCommand request, CancellationToken ct)
	{
		ExternalIdentityDetails? identity = await externalIdentityReadRepository.FindGoogleIdentityAsync(request.GoogleSubjectId, ct);

		if (identity is null)
		{
			return TypedResult<LoginWithGoogleResult>.Fail(ResultError.NotFound("Account", request.GoogleSubjectId));
		}

		// Case A: User is already linked
		TenantId resolvedTenantId = identity.TenantId;

		// if Request specified a Tenant (e.g. subdomain), ensure it matches
		if (request.TenantId is { IsEmpty: false } tenantId && tenantId != resolvedTenantId)
		{
			return TypedResult<LoginWithGoogleResult>.Fail(
				ResultError.Conflict($"User belongs to tenant {resolvedTenantId}, not {request.TenantId}"));
		}

		// User has logged in before
		AccountDetails? accountDetails = await accountReadRepository.GetByIdAsync(identity.AccountId, ct);
		if (accountDetails == null)
		{
			return TypedResult<LoginWithGoogleResult>.Fail(ResultError.NotFound("Account",
				request.GoogleSubjectId));
		}

		var accountValidationResult = loginValidationService.EnsureCanLoginByAccountState(accountDetails.State);
		if (!accountValidationResult.IsSuccess)
		{
			return TypedResult<LoginWithGoogleResult>.Fail(ResultError.Validation(accountValidationResult.Error.Message));
		}

		// check tenant availability
		TenantSummary? tenantInfo = await tenantReadRepository.GetSummaryByIdAsync(resolvedTenantId, ct);
		if (tenantInfo == null)
		{
			return TypedResult<LoginWithGoogleResult>.Fail(ResultError.NotFound("Tenant", resolvedTenantId));
		}

		var tenantValidationResult = loginValidationService.EnsureCanLoginByTenantState(tenantInfo.State);
		if (!tenantValidationResult.IsSuccess)
		{
			return TypedResult<LoginWithGoogleResult>.Fail(ResultError.Validation(tenantValidationResult.Error.Message));
		}

		return TypedResult<LoginWithGoogleResult>.Ok(
			new LoginWithGoogleResult(
				accountDetails.Id,
				accountDetails.TenantId,
				accountDetails.Role,
				accountDetails.State));
	}
}
