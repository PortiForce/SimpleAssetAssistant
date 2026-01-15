using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;

public interface IPlatformRepository : IRepository<Core.Assets.Models.Platform, PlatformId>
{
	public Task UpdateAsync(Core.Assets.Models.Platform platform, CancellationToken ct);
}
