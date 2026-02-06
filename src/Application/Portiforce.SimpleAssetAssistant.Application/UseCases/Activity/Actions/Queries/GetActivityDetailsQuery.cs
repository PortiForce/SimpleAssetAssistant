using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Queries;

public sealed record GetActivityDetailsQuery(
	ActivityId Id,
	TenantId TenantId,
	AccountId AccountId) : IQuery<ActivityDetails>
{
}
