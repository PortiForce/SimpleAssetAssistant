using Portiforce.SAA.Application.Interfaces.Persistence.Activity;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Activity.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Activity.Projections;

namespace Portiforce.SAA.Application.UseCases.Activity.Handlers.Queries;

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