using Portiforce.SimpleAssetAssistant.Application.Interfaces.Guards;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Activity;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Services.Asset;
using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Flow.Factories;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Flow.Rules;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Projections;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Futures;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Activities.Rules;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public sealed class RegisterTradeCommandHandler(
	IActivityIdempotencyGuard activityIdempotencyGuard,
	IAssetLookupService assetLookupService,
	IActivityPersistenceService activityPersistenceService
) : IRequestHandler<RegisterTradeCommand, BaseCreateCommandResult<ActivityId>>
{
	public async ValueTask<BaseCreateCommandResult<ActivityId>> Handle(RegisterTradeCommand request, CancellationToken ct)
	{
		// 0) Validation and preparation
		ActivityGuards.EnsureFeeConsistency(request.FeeAmount, request.FeeAssetId);

		await activityIdempotencyGuard.EnsureNotExistsAsync(
			request.Metadata,
			AssetActivityKind.Trade,
			request.TenantId,
			request.PlatformAccountId, ct);

		HashSet<AssetId> assetIds = [request.InAssetId, request.OutAssetId];
		if (request is { FeeAssetId: not null, FeeAmount: not null })
		{
			assetIds.Add(request.FeeAssetId.Value);
		}

		IReadOnlyDictionary<AssetId, AssetListItem> fetchedAssetsMap = await assetLookupService.GetRequiredAsync(
			assetIds.ToList(),
			ct);

		AssetListItem outAsset = fetchedAssetsMap[request.OutAssetId];
		AssetListItem inAsset = fetchedAssetsMap[request.InAssetId];

		byte? feeAssetDecimals = null;
		if (request is { FeeAssetId: not null, FeeAmount: not null })
		{
			AssetListItem feeAsset = fetchedAssetsMap[request.FeeAssetId.Value];
			feeAssetDecimals = feeAsset.NativeDecimals;
		}

		AssetActivityReason actualReason = ActivityReasonRules.DetermineFromKinds(outAsset.Kind, inAsset.Kind);

		string extPrimaryId = request.Metadata.GetPrimaryId();
		ActivityReasonRules.EnsureIsTradeReason(actualReason, extPrimaryId);

		// 1) Build legs
		List<AssetMovementLeg> legs = MovementLegFactory.CreateSpotTwoLegsWithOptionalFee(
			outAsset.Id,
			request.OutAmount,
			outAsset.NativeDecimals,
			inAsset.Id,
			request.InAmount,
			inAsset.NativeDecimals,
			request.FeeAssetId,
			request.FeeAmount,
			feeAssetDecimals);

		// 2) Futures descriptor (MVP)
		FuturesDescriptor? futures = request.MarketKind == MarketKind.Futures
			? new FuturesDescriptor { InstrumentKey = "PForce stub" }
			: null;

		// 3) Build domain entity
		TradeActivity tradeActivity = TradeActivity.Create(
			tenantId: request.TenantId,
			platformAccountId: request.PlatformAccountId,
			occurredAt: request.OccurredAt,
			reason: actualReason,
			marketKind: request.MarketKind,
			executionType: request.ExecutionType,
			legs: legs,
			futures: futures,
			externalMetadata: request.Metadata,
			id: null);

		// 4) Persist data
		return await activityPersistenceService.PersistNewAsync(tradeActivity, extPrimaryId, ct);
	}
}
