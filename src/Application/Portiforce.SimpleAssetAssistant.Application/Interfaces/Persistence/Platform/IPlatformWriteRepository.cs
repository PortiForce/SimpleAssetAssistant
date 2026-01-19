using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Platform;

public interface IPlatformWriteRepository : IWriteRepository<Core.Assets.Models.Platform, PlatformId>
{
	Task UpdateAsync(Core.Assets.Models.Platform platform, CancellationToken ct);
}
