using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Activity.Requests.Activity;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Activity.Mappers;

public static class TradeMapper
{
	public static RegisterTradeCommand ToCommand(this RegisterTradeRequest r, AccountId accountId, TenantId tenantId) => new(
		AccountId: accountId,
		TenantId: tenantId,
		PlatformAccountId: PlatformAccountId.From(r.PlatformAccountId),
		OccurredAt: r.OccurredAt,
		MarketKind: r.MarketKind,
		ExecutionType: r.ExecutionType,
		InAssetId: AssetId.From(r.InAssetId),
		InAmount: Quantity.Create(r.InAmount),
		OutAssetId: AssetId.From(r.OutAssetId),
		OutAmount: Quantity.Create(r.OutAmount),
		FeeAssetId: r.FeeAssetId is null ? null : AssetId.From(r.FeeAssetId.Value),
		FeeAmount: r.FeeAmount is null ? null : Quantity.Create(r.FeeAmount.Value),
		Metadata: new ExternalMetadata(source: r.Source, externalId: r.ExternalId),
		CompletionType: r.CompletionType
	);
}
