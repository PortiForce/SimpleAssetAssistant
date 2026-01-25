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
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Handlers.Commands;

public sealed class RegisterExchangeCommandHandler(
	IActivityIdempotencyGuard activityIdempotencyGuard,
	IAssetLookupService assetLookupService,
	IActivityPersistenceService activityPersistenceService
) : IRequestHandler<RegisterExchangeCommand, CommandResult<ActivityId>>
{
	public async ValueTask<CommandResult<ActivityId>> Handle(RegisterExchangeCommand request, CancellationToken ct)
	{
		// 0) Validation and preparation
		ActivityCommandGuards.EnsureFeeConsistency(request.FeeAmount, request.FeeAssetId);
		ActivityCommandGuards.EnsureTradeOrExchangeShape(request.OutAmount, request.InAmount);

		await activityIdempotencyGuard.EnsureNotExistsAsync(
			request.Metadata,
			AssetActivityKind.Exchange,
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
		ActivityReasonRules.EnsureIsExchangeReason(actualReason, primaryId);

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

		// 3) Domain entity
		var exchangeActivity = ExchangeActivity.Create(
			tenantId: request.TenantId,
			platformAccountId: request.PlatformAccountId,
			occurredAt: request.OccurredAt,
			reason: actualReason,
			exchangeType: request.Type,
			legs: legs,
			externalMetadata: request.Metadata,
			completionType: request.CompletionType,
			id: activityId);

		return await activityPersistenceService.PersistNewAsync(exchangeActivity, primaryId, ct);
	}
}