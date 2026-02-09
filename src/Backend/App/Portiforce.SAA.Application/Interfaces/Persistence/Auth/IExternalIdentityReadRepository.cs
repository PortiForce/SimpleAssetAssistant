using Portiforce.SAA.Application.UseCases.Auth.Projections;

namespace Portiforce.SAA.Application.Interfaces.Persistence.Auth;

public interface IExternalIdentityReadRepository
{
	Task<ExternalIdentityDetails?> FindGoogleIdentityAsync(string googleUserExternalId, CancellationToken ct);
}
