using Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Invite;

public interface IInviteSummaryRepository
{
	Task<InviteSummary> GetSummaryAsync(
		TenantId requestTenantId,
		HashSet<InviteChannel>? requestChannels,
		HashSet<InviteState>? requestStates,
		DateTime? requestFromDate,
		DateTime? requestToDate,
		bool? requestHasAccount,
		bool? requestIncludeRevoked,
		InviteSummaryRange range,
		InviteTrendBucket trendBucket,
		CancellationToken ct);
}