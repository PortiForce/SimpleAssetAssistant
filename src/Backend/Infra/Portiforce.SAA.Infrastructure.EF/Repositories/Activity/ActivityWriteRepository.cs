using Portiforce.SAA.Application.Interfaces.Persistence.Activity;
using Portiforce.SAA.Core.Activities.Models.Actions;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Activity;

internal sealed class ActivityWriteRepository(AssetAssistantDbContext db) : IActivityWriteRepository
{
	public Task AddAsync(AssetActivityBase activity, CancellationToken ct) =>
		db.Activities.AddAsync(activity, ct).AsTask();

	public Task UpdateAsync(Core.Assets.Models.PlatformAccount platformAccount, CancellationToken ct) =>
		throw new NotSupportedException("Update operation is not supported");
}