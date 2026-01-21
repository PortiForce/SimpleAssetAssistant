using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Profile;

internal sealed class CurrentUserReadRepository : ICurrentUserReadRepository
{
	public Task<CurrentUserDetails?> GetByIdAsync(AccountId id, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
