using Portiforce.SimpleAssetAssistant.Core.Primitives;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;

public interface IPlatformRepository : IRepository<Core.Assets.Models.Platform, PlatformId>
{
	Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
	public Task UpdateAsync(Core.Assets.Models.Platform platform, CancellationToken ct);
}
