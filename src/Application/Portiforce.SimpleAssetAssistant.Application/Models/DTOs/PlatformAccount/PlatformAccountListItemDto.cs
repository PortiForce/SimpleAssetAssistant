using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.PlatformAccount;

public sealed record PlatformAccountListItemDto(
	PlatformAccountId Id,
	TenantId TenantId,
	AccountId AccountId,
	PlatformId PlatformId,
	string AccountName,
	string? ExternalUserId);
