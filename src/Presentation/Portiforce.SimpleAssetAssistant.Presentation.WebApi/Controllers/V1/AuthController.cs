using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Auth;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1;

[ApiController]
[Route("api/auth")]
public class AuthController(
	IMediator mediator,
	ILogger<AuthController> logger) : ControllerBase
{
	private const string TenantHeaderName = "X-Tenant-ID";

	[AllowAnonymous]
	[HttpPost("login/google")]
	public async Task<ActionResult<AuthResponse>> LoginGoogle(
		[FromBody] LoginWithGoogleRequest request,
		CancellationToken ct)
	{
		TenantId? tenantId = GetTenantFromHeader(out var problem);
		if (problem is not null)
		{
			return BadRequest(problem);
		}

		LoginWithGoogleCommand command = new LoginWithGoogleCommand(request.IdToken, tenantId);
		AuthResponse result = await mediator.Send(command, ct);

		return Ok(result);
	}

	/// <summary>
	/// Helper to safely extract and parse the Tenant ID from the X-Tenant-ID header.
	/// </summary>
	private TenantId? GetTenantFromHeader(out ProblemDetails? problem)
	{
		problem = null;

		if (!Request.Headers.TryGetValue(TenantHeaderName, out var values))
		{
			// Header not present -> try Global Login (if acceptable)
			return null; 
		}

		var headerValue = values.FirstOrDefault();
		if (string.IsNullOrWhiteSpace(headerValue))
		{
			return null;
		}

		try
		{
			TenantId tenantId = TenantId.From(Guid.Parse(headerValue));
			return tenantId;
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to parse X-Tenant-ID header: {HeaderValue}", headerValue);

			problem = new ProblemDetails
			{
				Title = "Invalid tenant header",
				Detail = $"{TenantHeaderName} must be a valid GUID.",
				Status = StatusCodes.Status400BadRequest,
				Extensions =
				{
					["code"] = "PF-400-INVALID_TENANT_HEADER"
				}
			};
			return null;
		}
	}
}
