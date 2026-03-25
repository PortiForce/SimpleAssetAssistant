using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;

public sealed record GetInviteSummaryQuery(
	TenantId TenantId,
	HashSet<InviteState>? States,
	HashSet<InviteChannel>? Channels,
	DateTime? FromDate,
	DateTime? ToDate,
	bool? HasAccount,
	bool? IncludeRevoked,
	InviteSummaryRange Range,
	InviteTrendBucket TrendBucket) : IQuery<InviteSummary>;