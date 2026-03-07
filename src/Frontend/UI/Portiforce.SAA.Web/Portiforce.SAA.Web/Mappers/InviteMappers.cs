using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Contracts.Enums;
using Portiforce.SAA.Contracts.Models.Invite;
using Portiforce.SAA.Core.Identity.Enums;

using InviteChannel = Portiforce.SAA.Contracts.Enums.InviteChannel;

namespace Portiforce.SAA.Web.Mappers;

public static class InviteMappers
{
	public static InviteAccountTier ToPresentation(this AccountTier domainTier) => domainTier switch
	{
		AccountTier.Observer => InviteAccountTier.Observer,
		AccountTier.Investor => InviteAccountTier.Investor,
		AccountTier.Strategist => InviteAccountTier.Strategist,
		_ => InviteAccountTier.None
	};

	public static AccountTier ToBusiness(this InviteAccountTier presentationTier) => presentationTier switch
	{
		InviteAccountTier.Observer => AccountTier.Observer,
		InviteAccountTier.Investor => AccountTier.Investor,
		InviteAccountTier.Strategist => AccountTier.Strategist,
		_ => throw new ArgumentOutOfRangeException(nameof(presentationTier), $"Unsupported tier: {presentationTier}")
	};

	public static InviteTenantRole ToPresentation(this Role domainRole) => domainRole switch
	{
		Role.TenantUser => InviteTenantRole.TenantUser,
		Role.TenantAdmin => InviteTenantRole.TenantAdmin,
		_ => InviteTenantRole.None
	};

	public static Role ToBusiness(this InviteTenantRole presentationRole) => presentationRole switch
	{
		InviteTenantRole.TenantUser => Role.TenantUser,
		InviteTenantRole.TenantAdmin => Role.TenantAdmin,
		_ => throw new ArgumentOutOfRangeException(nameof(presentationRole), $"Unsupported role: {presentationRole}")
	};

	public static InviteChannel ToPresentation(this Core.Identity.Enums.InviteChannel domainChannel) => domainChannel switch
	{
		Core.Identity.Enums.InviteChannel.Email => InviteChannel.Email,
		Core.Identity.Enums.InviteChannel.Telegram => InviteChannel.Telegram,
		Core.Identity.Enums.InviteChannel.AppleId => InviteChannel.AppleId,
		_ => InviteChannel.None
	};

	public static Core.Identity.Enums.InviteChannel ToBusiness(this InviteChannel presentationChannel) => presentationChannel switch
	{
		InviteChannel.Email => Core.Identity.Enums.InviteChannel.Email,
		InviteChannel.Telegram => Core.Identity.Enums.InviteChannel.Telegram,
		InviteChannel.AppleId => Core.Identity.Enums.InviteChannel.AppleId,
		_ => throw new ArgumentOutOfRangeException(nameof(presentationChannel), $"Unsupported channel: {presentationChannel}")
	};

	public static InviteStatus ToPresentation(this InviteState domainState) => domainState switch
	{
		InviteState.Created => InviteStatus.Pending,
		InviteState.Sent => InviteStatus.Pending,
		InviteState.Accepted => InviteStatus.Accepted,
		InviteState.RevokedByTenant => InviteStatus.Revoked,
		InviteState.DeclinedByUser => InviteStatus.Declined,
		InviteState.AcceptAttemptFailed => InviteStatus.Failed,
		_ => InviteStatus.Unknown
	};

	public static InviteState ToBusiness(this InviteStatus presentationState) => presentationState switch
	{
		InviteStatus.Pending => InviteState.Created,
		InviteStatus.Accepted => InviteState.Accepted,
		InviteStatus.Revoked => InviteState.RevokedByTenant,
		InviteStatus.Declined => InviteState.DeclinedByUser,
		InviteStatus.Failed => InviteState.AcceptAttemptFailed,
		_ => throw new ArgumentOutOfRangeException(nameof(presentationState), $"Unsupported status: {presentationState}")
	};

	public static InviteListResponse MapToInviteList(this PagedResult<InviteListItem> model)
	{
		InviteListItemResponse[] items = model.Items
			.Select(MapInviteListItem)
			.ToArray();

		return new InviteListResponse(
			items,
			model.TotalCount,
			model.PageNumber,
			model.PageSize);
	}

	private static InviteListItemResponse MapInviteListItem(this InviteListItem model)
	{
		return new InviteListItemResponse(
			model.Id,
			model.TenantId.Value,
			model.InviteTargetValue,
			model.InviteChannel.ToPresentation(),
			model.InviteTier.ToPresentation(),
			model.InviteRole.ToPresentation(),
			model.State.ToPresentation(),
			model.CreatedAtUtc,
			model.ExpiresAtUtc,
			model.InvitedBy.Value,
			model.AcceptedAtUtc,
			model.RelatedAccountId?.Value);
	}

	public static InviteDetailsResponse MapToInviteDetails(this InviteDetails model)
	{
		return new InviteDetailsResponse(
			model.Id,
			model.TenantId.Value,
			model.InviteTargetValue,
			model.InviteChannel.ToPresentation(),
			model.InviteTier.ToPresentation(),
			model.InviteRole.ToPresentation(),
			model.State.ToPresentation(),
			model.CreatedAtUtc,
			model.ExpiresAtUtc,
			model.InvitedBy.Value,
			model.SendTimesCount,
			model.AcceptedAtUtc,
			model.RelatedAccountId?.Value);
	}
}
