using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

public sealed class RegisterExchangeCommandHandler(
	IActivityWriteRepository activityRepository,
	IAssetReadRepository assetRepository,
	IUnitOfWork unitOfWork
) : IRequestHandler<RegisterExchangeCommand, BaseCreateCommandResult<ActivityId>>
{
	public async ValueTask<BaseCreateCommandResult<ActivityId>> Handle(RegisterExchangeCommand request, CancellationToken ct)
	{
		static bool IsFiatOrStableKind(AssetKind kind)
			=> kind is AssetKind.Fiat or AssetKind.Stablecoin;

		var assetIds = new HashSet<AssetId> { request.InAssetId, request.OutAssetId };
		if (request.FeeAssetId.HasValue)
		{
			assetIds.Add(request.FeeAssetId.Value);
		}

		var assetList = await assetRepository.GetListByAssetIdsAsync(
			assetIds.ToList(),
			ct);

		var fetchedAssetsMap = assetList.Items.ToDictionary(a => a.Id);

		if (!fetchedAssetsMap.TryGetValue(request.OutAssetId, out var outAsset))
		{
			throw new NotFoundException("Asset", request.OutAssetId);
		}

		if (!fetchedAssetsMap.TryGetValue(request.InAssetId, out var inAsset))
		{
			throw new NotFoundException("Asset", request.InAssetId);
		}

		byte? feeAssetDecimals = null;
		if (request.FeeAssetId is null && request.FeeAmount is not null
			|| request.FeeAssetId is not null && request.FeeAmount is null)
		{
			throw new BadRequestException("FeeAssetId and FeeAmount must be provided together.");
		}

		if (request is { FeeAssetId: not null, FeeAmount: not null })
		{
			if (!fetchedAssetsMap.TryGetValue(request.FeeAssetId.Value, out var feeAsset))
			{
				throw new NotFoundException("Asset", request.FeeAssetId.Value);
			}

			feeAssetDecimals = feeAsset.NativeDecimals;
		}

		var outMoney = IsFiatOrStableKind(outAsset.Kind);
		var inMoney = IsFiatOrStableKind(inAsset.Kind);

		var reason =
			outMoney && !inMoney ? AssetActivityReason.Buy :
			!outMoney && inMoney ? AssetActivityReason.Sell :
			AssetActivityReason.Conversion;

		// 1) Build legs
		var legs = new List<AssetMovementLeg>
		{
			AssetMovementLeg.Create(
				assetId: request.OutAssetId,
				amount: request.OutAmount,
				role: MovementRole.Principal,
				direction: MovementDirection.Outflow,
				allocation: AssetAllocationType.Spot,
				nativeDecimals: outAsset.NativeDecimals),

			AssetMovementLeg.Create(
				assetId: request.InAssetId,
				amount: request.InAmount,
				role: MovementRole.Principal,
				direction: MovementDirection.Inflow,
				allocation: AssetAllocationType.Spot,
				nativeDecimals: inAsset.NativeDecimals),
		};

		if (request is { FeeAssetId: not null, FeeAmount: not null })
		{
			legs.Add(AssetMovementLeg.Create(
				assetId: request.FeeAssetId.Value,
				amount: request.FeeAmount.Value,
				role: MovementRole.Fee,
				direction: MovementDirection.Outflow,
				allocation: AssetAllocationType.Spot,
				nativeDecimals: feeAssetDecimals!.Value));
		}

		// 3) Domain entity
		var exchange = ExchangeActivity.Create(
			tenantId: request.TenantId,
			platformAccountId: request.PlatformAccountId,
			occurredAt: request.OccurredAt,
			reason: reason,
			exchangeType: request.Type,
			legs: legs,
			externalMetadata: request.Metadata,
			id: null);

		// 4) Persist
		await activityRepository.AddAsync(exchange, ct);
		var affected = await unitOfWork.SaveChangesAsync(ct);
		if (affected == 0)
		{
			throw new ConflictException("No changes were persisted (possible concurrency issue).");
		}

		return new BaseCreateCommandResult<ActivityId>
		{
			Id = exchange.Id,
			Message = "Trade registered successfully"
		};
	}
}
