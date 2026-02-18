using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Result;

public sealed record AcceptInviteResult(
	Guid InviteId,
	AccountId AccountId);
