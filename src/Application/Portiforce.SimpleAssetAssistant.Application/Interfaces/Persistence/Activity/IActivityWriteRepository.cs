using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;

public interface IActivityWriteRepository
{
	Task AddAsync(AssetActivityBase activity, CancellationToken ct);
}
