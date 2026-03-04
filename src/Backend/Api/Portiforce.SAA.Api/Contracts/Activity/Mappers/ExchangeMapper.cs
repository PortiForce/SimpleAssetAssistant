using Portiforce.SAA.Api.Contracts.Activity.Requests.Activity;
using Portiforce.SAA.Application.UseCases.Activity.Actions.Commands;
using Portiforce.SAA.Core.Activities.Models;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Api.Contracts.Activity.Mappers;

public static class ExchangeMapper
{
	public static RegisterExchangeCommand ToCommand(this RegisterExchangeRequest r, AccountId accountId, TenantId tenantId) => new(
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
		Type: r.ExchangeType,
		Metadata: new ExternalMetadata(source: r.Source, externalId: r.ExternalId),
		CompletionType:r.CompletionType
	);
}
