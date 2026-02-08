namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Portfolio.Requests;

public sealed record AddAssetsRequest(List<Guid> AssetIds);