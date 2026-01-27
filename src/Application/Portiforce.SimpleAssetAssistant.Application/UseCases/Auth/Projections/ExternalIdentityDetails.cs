using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;

public sealed record ExternalIdentityDetails(
	TenantId TenantId,
	AccountId AccountId)
{
}
