using Portiforce.SAA.Application.Interfaces.Persistence.Profile;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Profile;

internal sealed class CurrentUserReadRepository : ICurrentUserReadRepository
{
	public Task<CurrentUserDetails?> GetByIdAsync(AccountId id, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
