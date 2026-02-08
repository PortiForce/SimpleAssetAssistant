using Portiforce.SimpleAssetAssistant.Application.Exceptions;
using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Profile;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Queries;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Handlers.Queries;

internal sealed class GetAccountDetailsQueryHandler(
	IAccountReadRepository accountReadRepository)
	: IRequestHandler<GetAccountDetailsQuery, AccountDetails>
{
	public async ValueTask<AccountDetails> Handle(
		GetAccountDetailsQuery request,
		CancellationToken ct)
	{
		AccountDetails? user = await accountReadRepository.GetByIdAsync(request.Id, ct);

		// Ensure the user belongs to the requesting Tenant!
		if (user is null || user.TenantId != request.TenantId)
		{
			throw new NotFoundException("User", request.Id);
		}

		return user;
	}
}
