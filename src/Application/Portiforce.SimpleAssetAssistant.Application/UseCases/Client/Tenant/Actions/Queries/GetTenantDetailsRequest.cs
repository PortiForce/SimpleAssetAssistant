using Portiforce.SimpleAssetAssistant.Application.Models.DTOs.Client.Tenant;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Actions.Queries;

public sealed record GetTenantDetailsRequest(TenantId Id) : IQuery<TenantDetailsDto>
{
}
