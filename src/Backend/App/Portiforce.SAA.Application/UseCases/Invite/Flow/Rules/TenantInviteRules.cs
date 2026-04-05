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

		return IsManageable(state, expirationTime, utcNow);
	}

	public static bool CanBeRevoked(
		InviteState state,
		DateTimeOffset expirationTime,
		DateTimeOffset utcNow) =>
		IsManageable(state, expirationTime, utcNow);

	public static bool CanBeAccepted(
		InviteState state,
		bool? blockFutureInvitesForTarget,
		DateTimeOffset expirationTime,
		DateTimeOffset utcNow) =>
		IsManageable(state, expirationTime, utcNow);

	public static bool CanBeDeclined(
		InviteState state,
		DateTimeOffset expirationTime,
		DateTimeOffset utcNow) =>
		IsManageable(state, expirationTime, utcNow);

	private static bool IsManageable(
		InviteState state,
		DateTimeOffset expirationTime,
		DateTimeOffset utcNow) =>
		state is InviteState.Created or InviteState.Sent
		&& expirationTime > utcNow;
}