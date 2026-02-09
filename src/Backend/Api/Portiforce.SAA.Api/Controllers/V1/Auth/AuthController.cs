using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Api.Contracts.Auth.Requests;
using Portiforce.SAA.Api.Interfaces;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Auth.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Api.Controllers.V1.Auth;

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

		var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
		var userAgent = Request.Headers.UserAgent.ToString();

		var command = new RefreshTokenCommand(request.RefreshToken, ipAddress, userAgent);

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

		var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

		await mediator.Send(
			new LogoutCommand(
				request.RefreshToken,
				currentUser.Id,
				currentUser.TenantId,
				ipAddress),
			ct);

		return NoContent();
	}
}
