using Portiforce.SAA.Application.Interfaces.Persistence.Activity;
using Portiforce.SAA.Core.Activities.Models.Activities;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Activity;

internal sealed class ActivityWriteRepository : IActivityWriteRepository
{
	public Task AddAsync(AssetActivityBase activity, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
