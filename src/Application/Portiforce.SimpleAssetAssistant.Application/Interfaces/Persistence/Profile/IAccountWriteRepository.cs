using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Profile;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;

public interface IAccountWriteRepository : IWriteRepository<Account, AccountId>
{
}
