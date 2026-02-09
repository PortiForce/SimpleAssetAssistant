using Portiforce.SAA.Core.Identity.Models.Auth;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Auth;

public interface IExternalIdentityWriteRepository
{
	Task AddAsync(ExternalIdentity externalIdentity, CancellationToken ct);
}
