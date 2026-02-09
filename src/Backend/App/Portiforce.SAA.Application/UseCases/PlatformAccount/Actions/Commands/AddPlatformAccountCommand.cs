using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.PlatformAccount.Actions.Commands;

public sealed record AddPlatformAccountCommand(
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	string AccountName,
	string? ExternalUserId
) : ICommand<CommandResult<PlatformAccountId>>;
