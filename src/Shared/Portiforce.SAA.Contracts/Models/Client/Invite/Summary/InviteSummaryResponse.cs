using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite.Summary;

public sealed record InviteSummaryResponse(
	InviteSummaryRange Range,
	InviteTrendBucket TrendBucket,
	DateTimeOffset FromUtc,
	DateTimeOffset ToUtc,
	InviteSummaryCardsResponse Cards,
	InviteOutcomeBreakdownResponse OutcomeBreakdown,
	IReadOnlyList<InviteTierUsageResponse> TierUsage,
	InviteTrendResponse Trend);