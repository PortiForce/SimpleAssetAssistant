using Portiforce.SAA.Core.Identity.Models.Profile;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Profile;

public interface IAccountWriteRepository : IWriteRepository<Account, AccountId>
{
}
