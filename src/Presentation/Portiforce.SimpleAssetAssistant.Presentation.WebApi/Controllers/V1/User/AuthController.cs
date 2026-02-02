using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Auth;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Base;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
	ITenantIdServiceResolver tenantIdResolver,
	IMediator mediator,
	ILogger<AuthController> logger) : ControllerBase
{
	[AllowAnonymous]
	[HttpPost("login/google")]
	public async Task<ActionResult<AuthResponse>> LoginGoogle(
		[FromBody] LoginWithGoogleRequest request,
		CancellationToken ct)
	{
		TenantId? tenantId = tenantIdResolver.GetTenantFromHeader(out var problem);
		if (problem is not null)
		{
			return BadRequest(problem);
		}

		LoginWithGoogleCommand command = new LoginWithGoogleCommand(request.IdToken, tenantId);
		AuthResponse result = await mediator.Send(command, ct);

		return Ok(result);
	}
}
