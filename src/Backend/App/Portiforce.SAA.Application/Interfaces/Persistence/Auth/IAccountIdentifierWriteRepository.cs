using Portiforce.SAA.Core.Identity.Models.Auth;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Auth;

public interface IAccountIdentifierWriteRepository
{
	Task AddAsync(AccountIdentifier accountIdentifier, CancellationToken ct);
}