namespace Portiforce.SAA.Api.Contracts.Portfolio.Requests;

public sealed record AddAssetRequest(Guid AssetId, decimal TargetPercentage);
