using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Client.Tenant.Actions.Commands;

public sealed record CreateTenantCommand(
	string Name,
	Email AdminEmail,
	TenantPlan Plan
) : ICommand<CommandResult<TenantId>>;
