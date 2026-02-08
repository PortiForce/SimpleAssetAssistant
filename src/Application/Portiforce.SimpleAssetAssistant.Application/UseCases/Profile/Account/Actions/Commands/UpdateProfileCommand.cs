using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Commands;

public sealed record UpdateProfileCommand(
	AccountId Id,
	string Alias,
	PhoneNumber? PhoneNumber,
	Email? BackupEmail,
	string Locale,
	string DefaultCurrency
) : ICommand<CommandResult<AccountId>>;
