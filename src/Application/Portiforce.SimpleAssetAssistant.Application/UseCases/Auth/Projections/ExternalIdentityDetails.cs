using Portiforce.SimpleAssetAssistant.Core.Identity.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;

public sealed record ExternalIdentityDetails(
	ExternalIdentityId Id,
	TenantId TenantId,
	AccountId AccountId,
	AuthProvider Provider,
	bool IsPrimary);
