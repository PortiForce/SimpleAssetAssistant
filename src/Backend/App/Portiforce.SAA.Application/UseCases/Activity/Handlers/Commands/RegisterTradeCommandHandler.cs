using Portiforce.SAA.Application.Interfaces.Guards;
using Portiforce.SAA.Application.Interfaces.Services.Activity;
using Portiforce.SAA.Application.Interfaces.Services.Asset;
using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Activity.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Activity.Flow.Factories;
using Portiforce.SAA.Application.UseCases.Activity.Flow.Rules;
using Portiforce.SAA.Application.UseCases.Asset.Projections;
using Portiforce.SAA.Core.Activities.Enums;
using Portiforce.SAA.Core.Activities.Models.Activities;
using Portiforce.SAA.Core.Activities.Models.Futures;
using Portiforce.SAA.Core.Activities.Models.Legs;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Handlers.Commands;

public sealed class RegisterTradeCommandHandler(
	IActivityIdempotencyGuard activityIdempotencyGuard,
	IAssetLookupService assetLookupService,
	IActivityPersistenceService activityPersistenceService
) : IRequestHandler<RegisterTradeCommand, CommandResult<ActivityId>>
{
	public async ValueTask<CommandResult<ActivityId>> Handle(RegisterTradeCommand request, CancellationToken ct)
	{
		// 0) Validation and preparation
		ActivityCommandGuards.EnsureFeeConsistency(request.FeeAmount, request.FeeAssetId);
		ActivityCommandGuards.EnsureTradeOrExchangeShape(request.OutAmount, request.InAmount);

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
			assetIds,
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

		string primaryId = request.Metadata.GetPrimaryId();
		ActivityReasonRules.EnsureIsTradeReason(actualReason, primaryId);

		var activityId = ActivityId.New();

		// 1) Build legs
		List<AssetMovementLeg> legs = MovementLegFactory.CreateSpotTwoLegsWithOptionalFee(
			activityId,
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
			? FuturesDescriptor .Create(
				"PForce-Stub",
				FuturesContractKind.Dated,
				"BTC",
				"BTC",
				FuturesPositionEffect.Open)
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
			completionType: request.CompletionType,
			id: null);

		// 4) Persist data
		return await activityPersistenceService.PersistNewAsync(tradeActivity, primaryId, ct);
	}
}