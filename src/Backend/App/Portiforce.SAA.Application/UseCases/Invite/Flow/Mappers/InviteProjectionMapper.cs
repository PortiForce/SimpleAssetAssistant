using Portiforce.SAA.Application.UseCases.Invite.Flow.Rules;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Details;
using Portiforce.SAA.Core.Identity.Models.Invite;

namespace Portiforce.SAA.Application.UseCases.Invite.Flow.Mappers;

internal static class InviteProjectionMapper
{
	public static InviteListItem ToListItem(InviteListItemRaw raw, DateTimeOffset nowUtc)
	{
		return new InviteListItem(
			raw.Id,
			raw.TenantId,
			raw.InviteTargetValue,
			raw.InviteChannel,
			raw.InviteTargetKind,
			raw.InviteTier,
			raw.InviteRole,
			raw.State,
			raw.Alias,
			raw.CreatedAtUtc,
			raw.ExpiresAtUtc,
			raw.InvitedBy,
			raw.AcceptedAtUtc,
			raw.RelatedAccountId,
			raw.BlockFutureInvitesForTarget,
			TenantInviteRules.CanBeResent(raw.State, raw.BlockFutureInvitesForTarget, raw.ExpiresAtUtc, nowUtc),
			TenantInviteRules.CanBeRevoked(raw.State, raw.ExpiresAtUtc, nowUtc));
	}

	public static AdminInviteDetails ToAdminDetails(InviteDetailsRaw raw, DateTimeOffset nowUtc)
	{
		return new AdminInviteDetails(
			raw.Id,
			raw.TenantId,
			raw.InviteTargetValue,
			raw.InviteChannel,
			raw.InviteTargetKind,
			raw.InviteTier,
			raw.InviteRole,
			raw.State,
			raw.Alias,
			raw.CreatedAtUtc,
			raw.ExpiresAtUtc,
			raw.InvitedBy,
			raw.SendTimesCount,
			raw.AcceptedAtUtc,
			raw.RelatedAccountId,
			raw.BlockFutureInvitesForTarget,
			TenantInviteRules.CanBeResent(raw.State, raw.BlockFutureInvitesForTarget, raw.ExpiresAtUtc, nowUtc),
			TenantInviteRules.CanBeRevoked(raw.State, raw.ExpiresAtUtc, nowUtc));
	}

	public static AdminInviteDetails ToAdminDetails(TenantInvite raw, DateTimeOffset nowUtc)
	{
		return new AdminInviteDetails(
			raw.Id,
			raw.TenantId,
			raw.InviteTarget.Value,
			raw.InviteTarget.Channel,
			raw.InviteTarget.Kind,
			raw.IntendedTier,
			raw.IntendedRole,
			raw.State,
			raw.Alias,
			raw.CreatedAtUtc,
			raw.ExpiresAtUtc,
			raw.InvitedByAccountId,
			raw.SendCount,
			raw.UpdatedAtUtc,
			raw.AcceptedAccountId,
			raw.BlockFutureInvites,
			TenantInviteRules.CanBeResent(raw.State, raw.BlockFutureInvites, raw.ExpiresAtUtc, nowUtc),
			TenantInviteRules.CanBeRevoked(raw.State, raw.ExpiresAtUtc, nowUtc));
	}

	public static OverviewInviteDetails ToOverviewDetails(TenantInvite raw, DateTimeOffset nowUtc)
	{
		return new OverviewInviteDetails(
			raw.Id,
			raw.TenantId,
			raw.InviteTarget.Value,
			raw.InviteTarget.Channel,
			raw.InviteTarget.Kind,
			raw.IntendedTier,
			raw.IntendedRole,
			raw.State,
			raw.CreatedAtUtc,
			raw.ExpiresAtUtc,
			raw.InvitedByAccountId,
			raw.SendCount,
			raw.UpdatedAtUtc,
			raw.AcceptedAccountId,
			raw.BlockFutureInvites,
			TenantInviteRules.CanBeAccepted(raw.State, raw.BlockFutureInvites, raw.ExpiresAtUtc, nowUtc),
			TenantInviteRules.CanBeDeclined(raw.State, raw.ExpiresAtUtc, nowUtc));
	}
}