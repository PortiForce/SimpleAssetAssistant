using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SimpleAssetAssistant.Application.Models.Auth;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Auth.Requests;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(
	ITenantIdServiceResolver tenantIdResolver,
	IMediator mediator,
	ILogger<AuthController> logger) : ControllerBase
{
	// todo 
	// 1. add login with passkeys

	[AllowAnonymous]
	[HttpPost("google")]
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

	[AllowAnonymous]
	[HttpPost("apple")]
	public async Task<ActionResult<AuthResponse>> LoginApple(
		[FromBody] LoginWithAppleRequest request,
		CancellationToken ct)
	{
		TenantId? tenantId = tenantIdResolver.GetTenantFromHeader(out var problem);
		if (problem is not null)
		{
			return BadRequest(problem);
		}

		var command = new LoginWithAppleCommand(request.IdToken, tenantId);
		var result = await mediator.Send(command, ct);
		return Ok(result);
	}

	[AllowAnonymous]
	[HttpPost("refresh")]
	public async Task<ActionResult<AuthResponse>> Refresh(
		[FromBody] RefreshTokenRequest request,
		CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request.RefreshToken))
		{
			return BadRequest("Refresh token is required.");
		}

		var command = new RefreshTokenCommand(request.RefreshToken);

		var result = await mediator.Send(command, ct);
		return Ok(result);
	}

	[HttpPost("logout")]
	[Authorize]
	public async Task<IActionResult> Logout(
		[FromServices] ICurrentUser currentUser,
		[FromBody] RefreshTokenRequest request,
		CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request.RefreshToken))
		{
			return BadRequest("Refresh token is required.");
		}
		await mediator.Send(
			new LogoutCommand(
				request.RefreshToken,
				currentUser.Id,
				currentUser.TenantId),
			ct);

		return NoContent();
	}
}
