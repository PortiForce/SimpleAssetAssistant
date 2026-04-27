using Portiforce.SAA.Application.Enums;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Messaging.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Messaging.Actions.Queries;

public sealed record GetInboxMessageListQuery(
	TenantId TenantId,
	string? Search,
	HashSet<InboxMessageState>? States,
	PageRequest PageRequest) : IQuery<PagedResult<InboxMessageListItem>>;
