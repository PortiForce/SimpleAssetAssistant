using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.PlatformAccount.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.PlatformAccount.Actions.Queries;

/// <summary>
/// Returns the user's connected accounts.
/// </summary>
/// <param name="AccountId"></param>
/// <param name="PageRequest"></param>
public sealed record GetPlatformAccountListQuery(
	AccountId AccountId,
	PageRequest PageRequest
) : IQuery<PagedResult<PlatformAccountListItem>>;
