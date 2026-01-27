using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Auth;

public interface IExternalIdentityReadRepository
{
	Task<ExternalIdentityDetails?> FindGoogleIdentityAsync(string googleUserExternalId, CancellationToken ct);
}
