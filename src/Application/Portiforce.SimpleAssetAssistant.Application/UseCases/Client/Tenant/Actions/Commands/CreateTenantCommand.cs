using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Actions.Commands;

public sealed record CreateTenantCommand(
	string Name,
	string Email,
	TenantPlan Plan
) : ICommand<BaseCreateCommandResponse<TenantId>>;
