using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Messaging;

namespace Portiforce.SAA.Api.Controllers.V1.Platform;

[ApiController]
[Route("api/v1/platform/{controller}")]
[Authorize]
public sealed class TenantsController(IMediator mediator) : ControllerBase
{
	// todo:
	// 1. load list of tenants (only for platform admins/ owner)
	// 2. load tenant details (only for platform admins/ owner)
	// 3. change tenant plan/ state (only for platform admins/ owner)

	[Authorize(Policy = "RequirePlatformAdmin")]
	[HttpGet]
	public Task<IActionResult> GetList(
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		// only for platform admins/ owner
		return Task.FromResult<IActionResult>(
			StatusCode(StatusCodes.Status501NotImplemented, "Tenant list is not implemented yet."));
	}

	[Authorize(Policy = "RequirePlatformAdmin")]
	[HttpGet("{tenantId:guid}")]
	public Task<IActionResult> GetById(
		[FromServices] ICurrentUser currentUser,
		[FromRoute] Guid tenantId,
		CancellationToken ct)
	{
		// only for platform admins/owner
		return Task.FromResult<IActionResult>(
			StatusCode(StatusCodes.Status501NotImplemented, "Tenant details is not implemented yet."));
	}
}
