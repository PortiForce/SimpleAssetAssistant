using Portiforce.SimpleAssetAssistant.Core.Identity.Models.Auth;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;

public interface IExternalIdentityWriteRepository
{
	Task AddAsync(ExternalIdentity externalIdentity, CancellationToken ct);
}
