using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Handlers.Commands;

public sealed class CreateAssetCommandHandler(
	IAssetReadRepository assetReadRepository,
	IAssetWriteRepository assetWriteRepository,
	IUnitOfWork unitOfWork
) : IRequestHandler<CreateAssetCommand, BaseCreateCommandResult<AssetId>>
{
	public async ValueTask<BaseCreateCommandResult<AssetId>> Handle(CreateAssetCommand request, CancellationToken ct)
	{
		// 1. Validate Business Rules (Uniqueness)
		// Domain entities enforce their own invariants, but "Uniqueness" is a set-based validation,
		// so it belongs in the Application Layer (Handler).
		bool exists = await assetReadRepository.ExistsByCodeAsync(request.Code, ct);
		if (exists)
		{
			throw new ConflictException($"Asset with code '{request.Code}' already exists.");
		}

		// 2. Parse: not used here as my decision is to have valid state of command model as long as they reached handler
		
		// 3. Construct missing values if needed

		// 4. Orchestrate Domain Logic
		// The Entity Factory handles internal consistency (e.g. name length).
		Core.Assets.Models.Asset asset = Core.Assets.Models.Asset.Create(
			code: request.Code,
			kind: request.AssetKind,
			state: AssetLifecycleState.Draft,
			name: request.Name,
			nativeDecimals: request.NativeDecimals
		);

		// 5. Persistence
		await assetWriteRepository.AddAsync(asset, ct);
		var affectedRecords = await unitOfWork.SaveChangesAsync(ct);
		if (affectedRecords == 0)
		{
			throw new ConflictException("No changes were persisted (possible concurrency issue or no-op update).");
		}

		// 6. Response
		return new BaseCreateCommandResult<AssetId>
		{
			Id = asset.Id,
			Message = "Asset created successfully."
		};
	}
}
