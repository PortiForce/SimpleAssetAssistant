using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Queries;

public sealed record GetAccountListQuery(
	TenantId TenantId,
	PageRequest PageRequest
) : IQuery<PagedResult<AccountListItem>>;
