namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Profile.Account.Requests;

public sealed record UpdateStrategyRequest(string RiskLevel, List<string> Goals);
