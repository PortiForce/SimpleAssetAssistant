using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Me;

[Route("api/v1/me/profile")]
[ApiController]
[Authorize]
public sealed class ProfileController(IMediator mediator) : ControllerBase
{
	//[HttpGet]
	//[ProducesResponseType(typeof(UserProfileDetails), StatusCodes.Status200OK)]
	//public async Task<IActionResult> GetMyProfile(CancellationToken ct)
	//{
	//	// Handler uses ICurrentUser to find the ID
	//	var result = await mediator.Send(new GetMyProfileQuery(), ct);
	//	return Ok(result);
	//}

	//[HttpPatch]
	//public async Task<IActionResult> UpdateMyProfile(
	//	[FromBody] UpdateProfileRequest request,
	//	CancellationToken ct)
	//{
	//	var command = new UpdateProfileCommand(request.Alias);
	//	await mediator.Send(command, ct);
	//	return NoContent();
	//}

	//[HttpPut("strategy")]
	//public async Task<IActionResult> UpdateStrategy(
	//	[FromBody] UpdateStrategyRequest request,
	//	CancellationToken ct)
	//{
	//	var command = new UpdateUserStrategyCommand(request.RiskLevel, request.Goals);
	//	await mediator.Send(command, ct);
	//	return NoContent();
	//}
}
