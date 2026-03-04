namespace Portiforce.SAA.Api.Contracts.Portfolio.Requests;

public sealed record AddAssetsRequest(List<Guid> AssetIds);