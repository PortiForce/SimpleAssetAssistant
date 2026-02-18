using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Profile;

internal sealed class CurrentUserWriteRepository(AssetAssistantDbContext db) : ICurrentUserWriteRepository
{
	public Task AddAsync(Account entity, CancellationToken ct)
	{
		throw new NotSupportedException("This operation is not supported");
	}
}
