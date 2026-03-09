using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Activity.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Activity.Actions.Queries;

public sealed record GetActivityDetailsQuery(
	ActivityId Id,
	TenantId TenantId,
	AccountId AccountId) : IQuery<ActivityDetails>
{
}
