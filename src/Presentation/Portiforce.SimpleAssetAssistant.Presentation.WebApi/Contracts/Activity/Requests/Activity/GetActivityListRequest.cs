namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Activity.Requests.Activity;

public sealed record GetActivityListRequest(
	int PageNumber = 1,
	int PageSize = 20,
	DateTimeOffset? FromDate = null,
	DateTimeOffset? ToDate = null,
	string? AssetCode = null
);
