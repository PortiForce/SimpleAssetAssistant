using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Models.DTOs.PlatformAccount;

public sealed record PlatformAccountDto(
	PlatformAccountId Id,
	TenantId TenantId,
	AccountId AccountId, // Owner
	PlatformId PlatformId,
	string AccountName, // User's custom name: "My Main Binance"
	string? ExternalUserId);
