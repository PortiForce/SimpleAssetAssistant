using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Interfaces;

public interface ITenantIdServiceResolver
{
	TenantId? GetTenantFromHeader(out ProblemDetails? problem);
}
