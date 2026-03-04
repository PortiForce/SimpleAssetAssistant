using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Profile;

public interface ICurrentUserReadRepository : IReadRepository<CurrentUserDetails, AccountId>
{

}
