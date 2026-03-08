using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;

public sealed record GetInviteListQuery(
	TenantId TenantId,
	string? Search,
	InviteState? State,
	InviteChannel? Channel,
	int Page = 1,
	int PageSize = 20): IQuery<PagedResult<InviteListItem>>;
