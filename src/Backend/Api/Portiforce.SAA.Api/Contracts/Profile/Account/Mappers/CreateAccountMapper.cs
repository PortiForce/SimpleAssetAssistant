using Portiforce.SAA.Api.Contracts.Profile.Account.Requests;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Api.Contracts.Profile.Account.Mappers;

public static class CreateAccountMapper
{
	public static CreateAccountCommand ToCommand(
		this InviteUserRequest request,
		TenantId tenantId) => new(
		tenantId,
		Email.Create(request.Email),
		request.Alias,
		request.Role,
		request.Tier);
}
