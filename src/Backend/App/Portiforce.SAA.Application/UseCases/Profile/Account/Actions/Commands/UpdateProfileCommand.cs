using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;

public sealed record UpdateProfileCommand(
	AccountId Id,
	string Alias,
	PhoneNumber? PhoneNumber,
	Email? BackupEmail,
	string Locale,
	string DefaultCurrency
) : ICommand<CommandResult<AccountId>>;
