namespace Portiforce.SAA.Api.Contracts.Profile.Account.Requests;

public sealed record UpdateStrategyRequest(string RiskLevel, List<string> Goals);
