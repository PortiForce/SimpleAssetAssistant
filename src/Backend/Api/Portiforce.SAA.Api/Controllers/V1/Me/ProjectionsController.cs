using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Messaging;

namespace Portiforce.SAA.Api.Controllers.V1.Me;


[ApiController]
[Route("api/v1/me/{controller}")]
[Authorize]
public sealed class ProjectionsController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public Task<IActionResult> GetSummary(
		[FromServices] ICurrentUser currentUser,
		[FromQuery] Guid? portfolioId,
		CancellationToken ct)
	{
		return Task.FromResult<IActionResult>(
			StatusCode(StatusCodes.Status501NotImplemented, "Projections summary is not implemented yet."));
	}

	[HttpGet("suggestions")]
	public Task<IActionResult> GetSuggestions(
		[FromServices] ICurrentUser currentUser,
		[FromQuery] Guid? portfolioId,
		CancellationToken ct)
	{
		return Task.FromResult<IActionResult>(
			StatusCode(StatusCodes.Status501NotImplemented, "Projections suggestion is not implemented yet."));
	}
}
