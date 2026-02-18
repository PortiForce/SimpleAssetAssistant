using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Client.Tenant.Actions.Commands;

public sealed record UpdateTenantCommand(
	TenantId Id,
	Email Email,
	TenantPlan Plan, 
	TenantState State
) : ICommand<TypedResult<TenantId>>;
