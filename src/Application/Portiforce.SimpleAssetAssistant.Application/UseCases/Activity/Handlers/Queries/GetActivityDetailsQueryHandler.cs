using Portiforce.SimpleAssetAssistant.Application.Interfaces.Persistence.Activity;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Queries;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Projections;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Handlers.Queries;

internal sealed class GetActivityDetailsQueryHandler(
	IActivityReadRepository repository)
	: IRequestHandler<GetActivityDetailsQuery, ActivityDetails?>
{
	public async ValueTask<ActivityDetails?> Handle(GetActivityDetailsQuery request, CancellationToken ct)
	{
		return await repository.GetDetailsAsync(
			request.Id,
			request.TenantId,
			request.AccountId,
			ct);
	}
}