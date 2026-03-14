using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;

public sealed record GetInviteListQuery(
	TenantId TenantId,
	string? Search,
	HashSet<InviteState>? States,
	HashSet<InviteChannel>? Channels,
	bool? HasAccount,
	PageRequest PageRequest) : IQuery<PagedResult<InviteListItem>>;
