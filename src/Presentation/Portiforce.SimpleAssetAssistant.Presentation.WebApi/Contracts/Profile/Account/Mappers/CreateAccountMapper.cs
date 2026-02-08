using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Profile.Account.Requests;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Profile.Account.Mappers;

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
