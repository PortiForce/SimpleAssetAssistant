using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Actions.Commands;

public sealed record UpdateTenantSettingsCommand(
	TenantId Id,
	string DefaultCurrency,
	bool EnforceTwoFactor
) : ICommand<BaseCommandResponse>;
