using Portiforce.SAA.Core.Activities.Models.Actions;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Activity;

public interface IActivityWriteRepository
{
	Task AddAsync(AssetActivityBase activity, CancellationToken ct);
}
