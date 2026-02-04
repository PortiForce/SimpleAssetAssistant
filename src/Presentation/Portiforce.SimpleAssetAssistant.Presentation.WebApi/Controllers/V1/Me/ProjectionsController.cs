using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Me;


[ApiController]
[Route("api/v1/me/projections")]
[Authorize]
public sealed class ProjectionsController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetSummary(
		[FromQuery] Guid? portfolioId,
		CancellationToken ct)
	{
		// var query = new GetProjectionsQuery(portfolioId.HasValue ? new PortfolioId(portfolioId.Value) : null);
		// var result = await mediator.Send(query, ct);
		// return Ok(result);
		throw new NotImplementedException();
	}

	[HttpGet("suggestions")]
	public async Task<IActionResult> GetSuggestions(
		[FromQuery] Guid? portfolioId,
		CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
