using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Actions.Commands;

public sealed record UpdateTenantCommand(
	TenantId Id,
	Email Email,
	TenantPlan Plan, 
	TenantState State
) : ICommand<CommandResult<TenantId>>;
