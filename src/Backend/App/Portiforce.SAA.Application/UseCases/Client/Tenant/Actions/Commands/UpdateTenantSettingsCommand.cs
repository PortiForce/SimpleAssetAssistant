using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Client.Tenant.Actions.Commands;

public sealed record UpdateTenantSettingsCommand(
	TenantId Id,
	string DefaultCurrency,
	bool EnforceTwoFactor
) : ICommand<BaseCommandResult>;
