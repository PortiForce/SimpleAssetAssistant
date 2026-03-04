using Portiforce.SAA.Core.Activities.Models.Activities;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Activity;

public interface IActivityWriteRepository
{
	Task AddAsync(AssetActivityBase activity, CancellationToken ct);
}
