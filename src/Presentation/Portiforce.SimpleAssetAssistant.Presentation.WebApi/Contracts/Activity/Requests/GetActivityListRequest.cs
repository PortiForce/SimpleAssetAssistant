namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Activity.Requests;

public sealed record GetActivityListRequest(
	Guid AccountId,
	int PageNumber = 1,
	int PageSize = 20,
	DateTimeOffset? FromDate = null,
	DateTimeOffset? ToDate = null,
	string? AssetCode = null
);
