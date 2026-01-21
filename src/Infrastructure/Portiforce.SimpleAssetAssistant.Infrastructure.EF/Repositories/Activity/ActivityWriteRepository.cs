using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;
using Portiforce.SimpleAssetAssistant.Core.Activities.Models.Activities;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Activity;

internal sealed class ActivityWriteRepository : IActivityWriteRepository
{
	public Task AddAsync(AssetActivityBase activity, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
