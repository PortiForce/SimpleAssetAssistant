using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;
using Portiforce.SAA.Contracts.Enums;
using Portiforce.SAA.Contracts.Models.Client.Invite;
using Portiforce.SAA.Contracts.Models.Client.Invite.Summary;
using Portiforce.SAA.Core.Identity.Enums;

using InviteChannel = Portiforce.SAA.Contracts.Enums.InviteChannel;
using InviteSummaryRange = Portiforce.SAA.Application.UseCases.Invite.Projections.Summary.InviteSummaryRange;
using InviteTrendBucket = Portiforce.SAA.Application.UseCases.Invite.Projections.Summary.InviteTrendBucket;

namespace Portiforce.SAA.Web.Mappers;

public static class InviteMapper
{
	public static InviteAccountTier ToPresentation(this AccountTier domainTier) => domainTier switch
	{
		AccountTier.Observer => InviteAccountTier.Observer,
		AccountTier.Investor => InviteAccountTier.Investor,
		AccountTier.Strategist => InviteAccountTier.Strategist,
		_ => InviteAccountTier.None
	};

	public static HashSet<AccountTier> ToBusinessSet(this InviteAccountTier[] presentationTiers)
	{
		HashSet<AccountTier> set = [];
		foreach (InviteAccountTier inviteAccountTier in presentationTiers)
		{
			AccountTier? mappedValue = inviteAccountTier.ToBusiness();
			if (mappedValue != null)
			{
				_ = set.Add(mappedValue.Value);
			}
		}

		return set;
	}

	public static AccountTier? ToBusiness(this InviteAccountTier presentationTier) => presentationTier switch
	{
		InviteAccountTier.Observer => AccountTier.Observer,
		InviteAccountTier.Investor => AccountTier.Investor,
		InviteAccountTier.Strategist => AccountTier.Strategist,
		_ => null
	};

	public static InviteTenantRole ToPresentation(this Role domainRole) => domainRole switch
	{
		Role.TenantUser => InviteTenantRole.TenantUser,
		Role.TenantAdmin => InviteTenantRole.TenantAdmin,
		_ => InviteTenantRole.None
	};

	public static HashSet<Role> ToBusinessSet(this InviteTenantRole[] presentationRoles)
	{
		HashSet<Role> set = [];
		foreach (InviteTenantRole inviteTenantRole in presentationRoles)
		{
			Role? mappedValue = inviteTenantRole.ToBusiness();
			if (mappedValue != null)
			{
				_ = set.Add(mappedValue.Value);
			}
		}

		return set;
	}

	public static Role? ToBusiness(this InviteTenantRole presentationRole) => presentationRole switch
	{
		InviteTenantRole.TenantUser => Role.TenantUser,
		InviteTenantRole.TenantAdmin => Role.TenantAdmin,
		_ => null
	};

	public static InviteChannel ToPresentation(this Core.Identity.Enums.InviteChannel domainChannel) =>
		domainChannel switch
		{
			Core.Identity.Enums.InviteChannel.Email => InviteChannel.Email,
			Core.Identity.Enums.InviteChannel.Telegram => InviteChannel.Telegram,
			Core.Identity.Enums.InviteChannel.AppleId => InviteChannel.AppleId,
			_ => InviteChannel.None
		};

	public static HashSet<Core.Identity.Enums.InviteChannel> ToBusinessSet(this InviteChannel[] presentationChannels)
	{
		HashSet<Core.Identity.Enums.InviteChannel> set = [];
		foreach (InviteChannel presentationChannel in presentationChannels)
		{
			Core.Identity.Enums.InviteChannel? mappedValue = presentationChannel.ToBusiness();
			if (mappedValue != null)
			{
				_ = set.Add(mappedValue.Value);
			}
		}

		return set;
	}

	public static Core.Identity.Enums.InviteChannel? ToBusiness(this InviteChannel presentationChannel) =>
		presentationChannel switch
		{
			InviteChannel.Email => Core.Identity.Enums.InviteChannel.Email,
			InviteChannel.Telegram => Core.Identity.Enums.InviteChannel.Telegram,
			InviteChannel.AppleId => Core.Identity.Enums.InviteChannel.AppleId,
			_ => null
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

	public static HashSet<InviteState> ToBusinessSet(this InviteStatus[] presentationStates)
	{
		HashSet<InviteState> set = [];
		foreach (InviteStatus presentationState in presentationStates)
		{
			InviteState? mappedValue = presentationState.ToBusiness();
			if (mappedValue != null)
			{
				_ = set.Add(mappedValue.Value);
			}
		}

		return set;
	}

	public static InviteState? ToBusiness(this InviteStatus presentationState) => presentationState switch
	{
		InviteStatus.Pending => InviteState.Created,
		InviteStatus.Accepted => InviteState.Accepted,
		InviteStatus.Revoked => InviteState.RevokedByTenant,
		InviteStatus.Declined => InviteState.DeclinedByUser,
		InviteStatus.Failed => InviteState.AcceptAttemptFailed,
		InviteStatus.Expired => InviteState.Expired,
		_ => null
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
			ToPresentation(model.InviteTier),
			model.InviteRole.ToPresentation(),
			model.State.ToPresentation(),
			model.CreatedAtUtc,
			model.ExpiresAtUtc,
			model.InvitedBy.Value,
			model.AcceptedAtUtc,
			model.RelatedAccountId?.Value,
			model.CanResend,
			model.CanRevoke);
	}

	public static InviteDetailsResponse MapToInviteDetails(this InviteDetails model)
	{
		return new InviteDetailsResponse(
			model.Id,
			model.TenantId.Value,
			model.InviteTargetValue,
			model.InviteChannel.ToPresentation(),
			ToPresentation(model.InviteTier),
			model.InviteRole.ToPresentation(),
			model.State.ToPresentation(),
			model.CreatedAtUtc,
			model.ExpiresAtUtc,
			model.InvitedBy.Value,
			model.SendTimesCount,
			model.AcceptedAtUtc,
			model.RelatedAccountId?.Value,
			model.CanResend,
			model.CanRevoke);
	}

	public static InviteSummaryRange ToBusiness(this Contracts.Enums.InviteSummaryRange item) => item switch
	{
		Contracts.Enums.InviteSummaryRange.Today => InviteSummaryRange.Today,
		Contracts.Enums.InviteSummaryRange.LastWeek => InviteSummaryRange.LastWeek,
		Contracts.Enums.InviteSummaryRange.LastMonth => InviteSummaryRange.LastMonth,
		Contracts.Enums.InviteSummaryRange.AllTime => InviteSummaryRange.AllTime,
		_ => throw new ArgumentOutOfRangeException(nameof(item), $"Not expected range value: {item}")
	};

	public static InviteTrendBucket ToBusiness(this Contracts.Enums.InviteTrendBucket item) => item switch
	{
		Contracts.Enums.InviteTrendBucket.Hour => InviteTrendBucket.Hour,
		Contracts.Enums.InviteTrendBucket.Day => InviteTrendBucket.Day,
		Contracts.Enums.InviteTrendBucket.Week => InviteTrendBucket.Week,
		Contracts.Enums.InviteTrendBucket.Month => InviteTrendBucket.Month,
		_ => throw new ArgumentOutOfRangeException(nameof(item), $"Not expected trend bucket value: {item}")
	};

	public static Contracts.Enums.InviteSummaryRange ToPresentation(this InviteSummaryRange item) => item switch
	{
		InviteSummaryRange.Today => Contracts.Enums.InviteSummaryRange.Today,
		InviteSummaryRange.LastWeek => Contracts.Enums.InviteSummaryRange.LastWeek,
		InviteSummaryRange.LastMonth => Contracts.Enums.InviteSummaryRange.LastMonth,
		InviteSummaryRange.AllTime => Contracts.Enums.InviteSummaryRange.AllTime,
		_ => throw new ArgumentOutOfRangeException(nameof(item), $"Not expected range value: {item}")
	};

	public static Contracts.Enums.InviteTrendBucket ToPresentation(this InviteTrendBucket item) => item switch
	{
		InviteTrendBucket.Hour => Contracts.Enums.InviteTrendBucket.Hour,
		InviteTrendBucket.Day => Contracts.Enums.InviteTrendBucket.Day,
		InviteTrendBucket.Week => Contracts.Enums.InviteTrendBucket.Week,
		InviteTrendBucket.Month => Contracts.Enums.InviteTrendBucket.Month,
		_ => throw new ArgumentOutOfRangeException(nameof(item), $"Not expected trend bucket value: {item}")
	};

	public static InviteSummaryResponse MapToInviteSummary(this InviteSummary model)
	{
		InviteTierUsageResponse[] tierUsage = model.TierUsage
			.Select(static item => new InviteTierUsageResponse(
				item.TierCode,
				item.Used,
				item.Left,
				item.TotalLimit,
				item.UsedPercent,
				item.LeftPercent))
			.ToArray();

		InviteTrendPointResponse[] trendPoints = model.Trend.Points
			.Select(item => new InviteTrendPointResponse(
				item.BucketStartUtc,
				item.Sent,
				item.Accepted,
				item.Declined,
				item.Failed,
				item.Pending,
				item.Expired,
				item.Revoked))
			.ToArray();

		return new InviteSummaryResponse(
			model.Range.ToPresentation(),
			model.TrendBucket.ToPresentation(),
			model.FromUtc,
			model.ToUtc,
			new InviteSummaryCardsResponse(
				model.Cards.TotalSent,
				model.Cards.Accepted,
				model.Cards.Declined,
				model.Cards.Failed,
				model.Cards.Pending,
				model.Cards.Expired,
				model.Cards.Revoked,
				model.Cards.AcceptanceRatePercent),
			new InviteOutcomeBreakdownResponse(
				model.OutcomeBreakdown.Accepted,
				model.OutcomeBreakdown.Declined,
				model.OutcomeBreakdown.Failed,
				model.OutcomeBreakdown.Pending,
				model.OutcomeBreakdown.Expired,
				model.OutcomeBreakdown.Revoked),
			tierUsage,
			new InviteTrendResponse(
				model.Trend.Bucket.ToPresentation(),
				trendPoints));
	}
}