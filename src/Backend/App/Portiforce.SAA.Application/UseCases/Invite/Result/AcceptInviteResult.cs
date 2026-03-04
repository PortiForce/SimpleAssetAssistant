using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Result;

public sealed record AcceptInviteResult(
	Guid InviteId,
	AccountId AccountId,
	TenantId TenantId,
	Role Role,
	AccountState State);
