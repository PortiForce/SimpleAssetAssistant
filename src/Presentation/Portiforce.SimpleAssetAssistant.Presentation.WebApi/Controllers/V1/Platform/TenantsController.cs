using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Platform;

[ApiController]
[Route("api/v1/platform/{controller}")]
[Authorize]
public sealed class TenantsController(IMediator mediator) : ControllerBase
{
	// todo:
	// 1. load list of tenants (only for platform admins/ owner)
	// 2. load tenant details (only for platform admins/ owner)
	// 3. change tenant plan/ state (only for platform admins/ owner)

	public async Task<IActionResult> GetList(CancellationToken ct)
	{
		// only for platform admins/ owner
		throw new NotImplementedException();
	}

	public async Task<IActionResult> GetById(Guid tenantId, CancellationToken ct)
	{
		// only for platform admins/owner
		throw new NotImplementedException();
	}
}
