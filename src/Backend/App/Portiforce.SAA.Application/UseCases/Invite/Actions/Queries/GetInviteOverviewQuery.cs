using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Details;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;

public sealed record GetInviteOverviewQuery(
	TenantId TenantId,
	string RawToken,
	ICurrentUser CurrentUser) : IQuery<TypedResult<OverviewInviteDetails>>;