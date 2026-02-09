using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Client.Tenant.Actions.Queries;

public sealed record GetTenantDetailsQuery(TenantId Id) : IQuery<TenantDetails>
{
}
