using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.PlatformAccount;
using Portiforce.SimpleAssetAssistant.Application.UseCases.PlatformAccount.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.PlatformAccount;

internal sealed class PlatformAccountReadRepository : IPlatformAccountReadRepository
{
	public Task<PlatformAccountDetails?> GetByIdAsync(PlatformAccountId id, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
