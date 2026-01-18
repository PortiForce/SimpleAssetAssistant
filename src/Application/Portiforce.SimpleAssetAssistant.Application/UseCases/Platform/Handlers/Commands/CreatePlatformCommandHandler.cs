using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;
using Portiforce.SimpleAssetAssistant.Application.Responses;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Core.Assets.Enums;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Platform.Handlers.Commands;

public sealed class CreatePlatformCommandHandler(
	IPlatformRepository platformRepository,
	IUnitOfWork unitOfWork
) : IRequestHandler<CreatePlatformCommand, BaseCreateCommandResponse<PlatformId>>
{
	public async ValueTask<BaseCreateCommandResponse<PlatformId>> Handle(CreatePlatformCommand request, CancellationToken ct)
	{
		// 1. Validate Business Rules (Uniqueness)
		// Domain entities enforce their own invariants, but "Uniqueness" is a set-based validation,
		// so it belongs in the Application Layer (Handler).
		bool exists = await platformRepository.ExistsByNameAsync(request.Name, ct);
		if (exists)
		{
			throw new ConflictException($"Platform with code '{request.Code}' already exists.");
		}

		// 2. Parse: not used here as my decision is to have valid state of command model as long as they reached handler

		// 3. Construct missing values if needed

		// 4. Orchestrate Domain Logic
		// The Entity Factory handles internal consistency (e.g. name length).
		Core.Assets.Models.Platform platform = Core.Assets.Models.Platform.Create(
			name: request.Name,
			code: request.Code,
			kind: request.Kind,
			state: PlatformState.Draft
		);

		// 5. Persistence
		await platformRepository.AddAsync(platform, ct);
		await unitOfWork.SaveChangesAsync(ct);

		// 6. Response
		return new BaseCreateCommandResponse<PlatformId>
		{
			Id = platform.Id,
			Message = "Platform created successfully."
		};
	}
}
