using Portiforce.SimpleAssetAssistant.Application.Interfaces.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.PlatformAccount.Projections;

public sealed record PlatformAccountDetails(
	PlatformAccountId Id,
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	string AccountName,
	string? ExternalUserId) : IDetailsProjection;
