using Portiforce.SimpleAssetAssistant.Core.Assets.Models;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.PllatformAccount;

public interface IPlatformAccountRepository : IRepository<PlatformAccount, PlatformAccountId>
{
	public Task UpdateAsync(PlatformAccount platformAccount, CancellationToken ct);
}
