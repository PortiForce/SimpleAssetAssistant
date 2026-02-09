using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Core.Identity.Models.Profile;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Profile;

internal sealed class AccountWriteRepository : IAccountWriteRepository
{
	public Task AddAsync(Account entity, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
