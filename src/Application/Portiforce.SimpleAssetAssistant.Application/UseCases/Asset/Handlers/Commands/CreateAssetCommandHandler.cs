using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Asset;
using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Asset.Handlers.Commands;

public sealed class CreateAssetCommandHandler(
	IAssetRepository assetRepository,
	IUnitOfWork unitOfWork
) : IRequestHandler<CreateAssetCommand, BaseCreateCommandResponse<AssetId>>
{
	public async ValueTask<BaseCreateCommandResponse<AssetId>> Handle(CreateAssetCommand request, CancellationToken ct)
	{
		// 1. Validate Business Rules (Uniqueness)
		// Domain entities enforce their own invariants, but "Uniqueness" is a set-based validation,
		// so it belongs in the Application Layer (Handler).
		bool exists = await assetRepository.ExistsByCodeAsync(request.Code, ct);
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
			name: request.Name,
			nativeDecimals: request.NativeDecimals
		);

		// 5. Persistence
		await assetRepository.AddAsync(asset, ct);
		await unitOfWork.SaveChangesAsync(ct);

		// 6. Response
		return new BaseCreateCommandResponse<AssetId>
		{
			Id = asset.Id,
			Message = "Asset created successfully."
		};
	}
}
