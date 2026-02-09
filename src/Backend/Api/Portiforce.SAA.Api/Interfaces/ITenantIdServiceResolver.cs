using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Api.Interfaces;

public interface ITenantIdServiceResolver
{
	TenantId? GetTenantFromHeader(out ProblemDetails? problem);
}
