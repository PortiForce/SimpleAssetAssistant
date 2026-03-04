using Portiforce.SAA.Application.UseCases.PlatformAccount.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.PlatformAccount;

public interface IPlatformAccountReadRepository : IReadRepository<PlatformAccountDetails, PlatformAccountId>
{
	
}
