using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Projections;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Futures;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Legs;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Handlers.Commands;

public sealed class RegisterTradeCommandHandler(
	IActivityWriteRepository activityRepository,
	IAssetReadRepository assetRepository,
	IUnitOfWork unitOfWork
) : IRequestHandler<RegisterTradeCommand, BaseCreateCommandResult<ActivityId>>
{
	public async ValueTask<BaseCreateCommandResult<ActivityId>> Handle(RegisterTradeCommand request, CancellationToken ct)
	{
		// 0. Fetch Required Asset Data and tech models
		var assetIds = new List<AssetId>
		{
			request.InAssetId,
			request.OutAssetId
		};
		if (request.FeeAssetId.HasValue && !assetIds.Contains(request.FeeAssetId.Value))
		{
			assetIds.Add(request.FeeAssetId.Value);
		}

		PageRequest pageRequest = new PageRequest(1, 20);		

		PagedResult<AssetListItem> assetList = await assetRepository.GetListByAssetIdsAsync(
			assetIds,
			pageRequest,
			ct
		);

		var outAssetDecimals = assetList.Items.FirstOrDefault(a => a.Id == request.OutAssetId)?.NativeDecimals ?? throw new NotFoundException(nameof(Asset), request.OutAssetId);
		var inAssetDecimals = assetList.Items.FirstOrDefault(a => a.Id == request.InAssetId)?.NativeDecimals ?? throw new NotFoundException(nameof(Asset), request.InAssetId);

		byte feeAssetDecimals = 0;
		if (request.FeeAssetId.HasValue)
		{
			feeAssetDecimals = assetList.Items.FirstOrDefault(a => a.Id == request.FeeAssetId.Value)?.NativeDecimals ?? throw new NotFoundException(nameof(Asset), request.FeeAssetId.Value);
		}

		// 1. Build the Legs (The Mapping Logic)
		var legs = new List<AssetMovementLeg>
		{
			// Leg 1: Outflow (Selling)
			AssetMovementLeg.Create(
				assetId: request.OutAssetId,
				amount: request.OutAmount,
				role: MovementRole.Principal,
				direction: MovementDirection.Outflow,
				allocation: AssetAllocationType.Spot,
				nativeDecimals: outAssetDecimals
			),
			// Leg 2: Inflow (Buying)
			AssetMovementLeg.Create(
				assetId: request.InAssetId,
				amount: request.InAmount,
				role: MovementRole.Principal,
				direction: MovementDirection.Inflow,
				allocation: AssetAllocationType.Spot,
				nativeDecimals: inAssetDecimals
			)
		};

		// Leg 3: Fee
		if (request is { FeeAssetId: not null, FeeAmount: not null })
		{
			legs.Add(AssetMovementLeg.Create(
				assetId: request.FeeAssetId.Value,
				amount: request.FeeAmount.Value,
				role: MovementRole.Fee,
				direction: MovementDirection.Outflow,
				allocation: AssetAllocationType.Spot,
				nativeDecimals: feeAssetDecimals
			));
		}

		// 2. Handle Futures Descriptor (if applicable)
		FuturesDescriptor? futures = null;
		if (request.MarketKind == MarketKind.Futures)
		{
			// tofo PForce: Simple mapping for MVP
			futures = new FuturesDescriptor
			{
				// todo feature: map other properties if present in command
				InstrumentKey = "PForce stub"
			};
		}

		// 3. Create Domain Entity
		TradeActivity trade = TradeActivity.Create(
			tenantId: request.TenantId,
			platformAccountId: request.PlatformAccountId,
			occurredAt: request.OccurredAt,
			// todo PForce: improve reason determination logic (if needed)
			reason: request.InAmount.Value > request.OutAmount.Value
					? AssetActivityReason.Buy
					: AssetActivityReason.Sell,
			marketKind: request.MarketKind,
			executionType: request.ExecutionType,
			legs: legs,
			futures: futures,
			externalMetadata: request.Metadata,
			id: null
		);

		// 4. Persist
		await activityRepository.AddAsync(trade, ct); 
		var affectedRecords = await unitOfWork.SaveChangesAsync(ct);
		if (affectedRecords == 0)
		{
			throw new ConflictException("No changes were persisted (possible concurrency issue or no-op update).");
		}

		return new BaseCreateCommandResult<ActivityId>
		{
			Id = trade.Id,
			Message = "Trade registered successfully"
		};
	}
}
