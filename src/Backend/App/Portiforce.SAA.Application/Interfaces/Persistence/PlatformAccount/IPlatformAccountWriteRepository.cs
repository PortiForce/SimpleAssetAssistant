using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.PlatformAccount;

public interface IPlatformAccountWriteRepository : IWriteRepository<Core.Assets.Models.PlatformAccount, PlatformAccountId>
{
	Task UpdateAsync(Core.Assets.Models.PlatformAccount platformAccount, CancellationToken ct);
}
