using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;

namespace Portiforce.SAA.Application.UseCases.Invite.Handlers.Queries;

public sealed class GetInviteSummaryQueryHandler(IInviteSummaryRepository inviteSummaryRepository)
	: IRequestHandler<GetInviteSummaryQuery, InviteSummary>
{
	public async ValueTask<InviteSummary> Handle(
		GetInviteSummaryQuery request,
		CancellationToken ct)
	{
		InviteSummary inviteSummary = await inviteSummaryRepository.GetSummaryAsync(
			request.TenantId,
			request.Channels,
			request.States,
			request.FromDate,
			request.ToDate,
			request.HasAccount,
			request.IncludeRevoked,
			request.Range,
			request.TrendBucket,
			ct);

		return inviteSummary;
	}
}