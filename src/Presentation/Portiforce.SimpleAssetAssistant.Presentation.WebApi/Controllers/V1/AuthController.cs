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

	[HttpPost("login/google")]
	public async Task<ActionResult<AuthResponse>> LoginGoogle([FromBody] LoginWithGoogleRequest request)
	{
		TenantId? tenantId = GetTenantFromHeader();

		LoginWithGoogleCommand command = new LoginWithGoogleCommand(request.IdToken, tenantId);

		var result = await mediator.Send(command);

		return Ok(result);
	}

	/// <summary>
	/// Helper to safely extract and parse the Tenant ID from the X-Tenant-ID header.
	/// </summary>
	private TenantId? GetTenantFromHeader()
	{
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
			var tenantId = TenantId.From(Guid.Parse(headerValue));
			return tenantId;
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to parse X-Tenant-ID header: {HeaderValue}", headerValue);

			// For login, ignoring it (treating as global) is usually safer than crashing.
			return null;
		}
	}
}
