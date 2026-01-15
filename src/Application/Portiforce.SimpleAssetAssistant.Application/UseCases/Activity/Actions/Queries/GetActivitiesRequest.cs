using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Activity;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Queries;

public sealed record GetActivitiesRequest(
	AccountId AccountId,
	PageRequest PageRequest,
	DateTimeOffset? FromDate = null,
	DateTimeOffset? ToDate = null,
	string? AssetCode = null      // Filter by specific asset
) : IQuery<PagedResult<ActivityListItemDto>>;