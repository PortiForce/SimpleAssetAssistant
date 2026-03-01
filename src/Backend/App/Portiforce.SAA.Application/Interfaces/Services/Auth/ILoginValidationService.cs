using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Application.Interfaces.Services.Auth;

public interface ILoginValidationService
{
	FlowResult.Result EnsureCanLoginByAccountState(AccountState accountState);

	FlowResult.Result EnsureCanLoginByTenantState(TenantState tenantState);
}
