using Portiforce.SimpleAssetAssistant.Application.UseCases.PlatformAccount.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.PlatformAccount;

public interface IPlatformAccountReadRepository : IReadRepository<PlatformAccountDetails, PlatformAccountId>
{
	
}
