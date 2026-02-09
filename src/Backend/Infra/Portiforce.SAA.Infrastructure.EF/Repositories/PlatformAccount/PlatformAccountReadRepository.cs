using Portiforce.SAA.Application.Interfaces.Persistence.PlatformAccount;
using Portiforce.SAA.Application.UseCases.PlatformAccount.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.PlatformAccount;

internal sealed class PlatformAccountReadRepository : IPlatformAccountReadRepository
{
	public Task<PlatformAccountDetails?> GetByIdAsync(PlatformAccountId id, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
