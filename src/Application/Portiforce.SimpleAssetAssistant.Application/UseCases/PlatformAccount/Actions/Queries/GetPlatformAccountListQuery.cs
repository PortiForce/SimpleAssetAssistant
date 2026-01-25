using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.PlatformAccount.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.PlatformAccount.Actions.Queries;

/// <summary>
/// Returns the user's connected accounts.
/// </summary>
/// <param name="AccountId"></param>
/// <param name="PageRequest"></param>
public sealed record GetPlatformAccountListQuery(
	AccountId AccountId,
	PageRequest PageRequest
) : IQuery<PagedResult<PlatformAccountListItem>>;
