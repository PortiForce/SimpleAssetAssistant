using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Auth.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Auth;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
	[HttpPost("login/google")]
	public async Task<ActionResult<AuthResponse>> LoginGoogle([FromBody] LoginWithGoogleRequest request)
	{
		// todo tech: fixMe - extract tenantId value
		TenantId tenantId = TenantId.New();

		LoginWithGoogleCommand command = new LoginWithGoogleCommand(request.IdToken, tenantId);

		var result = await mediator.Send(command);

		return Ok(result);
	}
}
