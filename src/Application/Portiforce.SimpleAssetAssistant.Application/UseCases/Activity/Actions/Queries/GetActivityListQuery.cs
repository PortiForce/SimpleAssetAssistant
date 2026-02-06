using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Queries;

public sealed record GetActivityListQuery(
	AccountId AccountId,
	TenantId TenantId,
	PageRequest PageRequest,
	DateTimeOffset? FromDate = null,
	DateTimeOffset? ToDate = null,
	string? AssetCode = null
) : IQuery<PagedResult<ActivityListItem>>;
