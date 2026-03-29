using Portiforce.SAA.Application.UseCases.Invite.Flow.Rules;
using Portiforce.SAA.Application.UseCases.Invite.Projections;

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
            raw.CreatedAtUtc,
            raw.ExpiresAtUtc,
            raw.InvitedBy,
            raw.AcceptedAtUtc,
            raw.RelatedAccountId,
            raw.BlockFutureInvitesForTarget,
            TenantInviteRules.CanBeResent(raw.State, raw.BlockFutureInvitesForTarget, raw.ExpiresAtUtc, nowUtc),
            TenantInviteRules.CanBeRevoked(raw.State, raw.ExpiresAtUtc, nowUtc));
    }

    public static InviteDetails ToDetails(InviteDetailsRaw raw, DateTimeOffset nowUtc)
    {
        return new InviteDetails(
            raw.Id,
            raw.TenantId,
            raw.InviteTargetValue,
            raw.InviteChannel,
            raw.InviteTargetKind,
            raw.InviteTier,
            raw.InviteRole,
            raw.State,
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
}