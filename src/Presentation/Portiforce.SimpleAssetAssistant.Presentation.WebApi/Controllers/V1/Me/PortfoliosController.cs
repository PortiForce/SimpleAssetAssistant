using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Me;


[ApiController]
[Route("api/v1/me/portfolios")]
[Authorize]
public sealed class PortfoliosController(IMediator mediator) : ControllerBase
{
	//[HttpGet]
	//[ProducesResponseType(typeof(List<PortfolioSummary>), StatusCodes.Status200OK)]
	//public async Task<IActionResult> GetList(CancellationToken ct)
	//{
	//	var result = await mediator.Send(new GetMyPortfoliosQuery(), ct);
	//	return Ok(result);
	//}

	//[HttpPost]
	//[ProducesResponseType(typeof(CommandResult<PortfolioId>), StatusCodes.Status201Created)]
	//public async Task<IActionResult> Create(
	//	[FromBody] CreatePortfolioRequest request,
	//	CancellationToken ct)
	//{
	//	var command = new CreatePortfolioCommand(request.Name, request.Description);
	//	var result = await mediator.Send(command, ct);

	//	return CreatedAtAction(
	//		nameof(GetById),
	//		new { id = result.Id.Value },
	//		result);
	//}

	//[HttpGet("{id:guid}")]
	//[ProducesResponseType(typeof(PortfolioDetails), StatusCodes.Status200OK)]
	//public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
	//{
	//	var result = await mediator.Send(new GetPortfolioDetailsQuery(new PortfolioId(id)), ct);
	//	return Ok(result);
	//}

	//[HttpPut("{id:guid}")]
	//public async Task<IActionResult> Update(
	//	[FromRoute] Guid id,
	//	[FromBody] UpdatePortfolioRequest request,
	//	CancellationToken ct)
	//{
	//	var command = new UpdatePortfolioCommand(new PortfolioId(id), request.Name, request.Description);
	//	await mediator.Send(command, ct);
	//	return NoContent();
	//}

	//[HttpPost("{id:guid}/assets")]
	//public async Task<IActionResult> AddAsset(
	//	[FromRoute] Guid id,
	//	[FromBody] AddAssetRequest request,
	//	CancellationToken ct)
	//{
	//	// Replaces "SetPriorityAsset"
	//	var command = new AddAssetToPortfolioCommand(
	//		new PortfolioId(id),
	//		new AssetId(request.AssetId),
	//		request.TargetPercentage);

	//	await mediator.Send(command, ct);
	//	return Ok();
	//}
}
