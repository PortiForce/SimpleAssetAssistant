using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.PlatformAccount;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.EF.Repositories.PlatformAccount;

internal sealed class PlatformAccountWriteRepository : IPlatformAccountWriteRepository
{
	public Task AddAsync(Core.Assets.Models.PlatformAccount entity, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	public Task UpdateAsync(Core.Assets.Models.PlatformAccount platformAccount, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
