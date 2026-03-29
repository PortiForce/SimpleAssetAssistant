using Portiforce.SAA.Core.Identity.Enums;

namespace Portiforce.SAA.Application.UseCases.Invite.Flow.Rules;

internal static class TenantInviteRules
{
	public static bool CanBeResent(
		InviteState state,
		bool? blockFutureInvitesForTarget,
		DateTimeOffset expirationTime,
		DateTimeOffset utcNow)
	{
		if (blockFutureInvitesForTarget.HasValue && blockFutureInvitesForTarget.Value)
		{
			return false;
		}

		return state is InviteState.Created or InviteState.Sent
			   && expirationTime > utcNow;
	}

	public static bool CanBeRevoked(
		InviteState state,
		DateTimeOffset expirationTime,
		DateTimeOffset utcNow)
	{
		return state is InviteState.Created or InviteState.Sent
			   && expirationTime > utcNow;
	}
}