using Portiforce.SAA.Application.Exceptions;
using Portiforce.SAA.Application.Interfaces.Services.Auth;
using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Application.UseCases.Auth.Flow.Services;

internal sealed class LoginValidationService : ILoginValidationService
{
	public FlowResult.Result EnsureCanLoginByAccountState(AccountState accountState)
	{
		// use only cases and states when person is able to log in
		bool canLogin = accountState is AccountState.Active or AccountState.PendingActivation;

		if (!canLogin)
		{
			return FlowResult.Result.Fail(ResultError.Validation($"User state is not valid for login flow {accountState}"));
		}

		return FlowResult.Result.Ok();
	}

	public FlowResult.Result EnsureCanLoginByTenantState(TenantState tenantState)
	{
		// use only cases and states when tenant is able to accept user logins
		bool canLogin = tenantState is TenantState.Active;

		if (!canLogin)
		{
			return FlowResult.Result.Fail(ResultError.Validation($"Tenant state is not valid for login flow {tenantState}"));
		}
		return FlowResult.Result.Ok();
	}
}
