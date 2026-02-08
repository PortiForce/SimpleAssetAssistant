namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Portfolio.Requests;

public sealed record AddAssetRequest(Guid AssetId, decimal TargetPercentage);
