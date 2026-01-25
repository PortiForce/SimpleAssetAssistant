using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.Profile;

internal sealed class AccountWriteRepository : IAccountWriteRepository
{
	public Task AddAsync(Account entity, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
