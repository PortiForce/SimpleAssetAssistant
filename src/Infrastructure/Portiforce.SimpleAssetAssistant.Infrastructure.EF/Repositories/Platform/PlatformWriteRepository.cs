using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Platform;

internal sealed class PlatformWriteRepository : IPlatformWriteRepository
{
	public Task AddAsync(Core.Assets.Models.Platform entity, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task UpdateAsync(Core.Assets.Models.Platform platform, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
