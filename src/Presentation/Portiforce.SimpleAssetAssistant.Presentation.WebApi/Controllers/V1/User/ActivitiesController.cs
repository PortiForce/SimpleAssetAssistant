using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Actions.Queries;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Activity.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Activity.Mappers;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Activity.Requests.Activity;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Base;


[ApiController]
[Route("api/v1/[controller]")]
public sealed class ActivitiesController(IMediator mediator) : ControllerBase
{
	[HttpPost("trades")]
	[ProducesResponseType(typeof(CommandResult<ActivityId>), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	public async Task<ActionResult<CommandResult<ActivityId>>> RegisterTrade(
		[FromBody] RegisterTradeRequest request,
		CancellationToken ct)
	{
		RegisterTradeCommand cmd = request.ToCommand();
		CommandResult<ActivityId> result = await mediator.Send(cmd, ct);

		if (result.Id.IsEmpty)
		{
			throw new InvalidOperationException("CommandResult.Id was null for successful RegisterTrade.");
		}

		return CreatedAtAction(
			actionName: nameof(GetById),
			routeValues: new { id = result.Id.Value },
			value: result);
	}

	[HttpPost("exchanges")]
	[ProducesResponseType(typeof(CommandResult<ActivityId>), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	public async Task<ActionResult<CommandResult<ActivityId>>> RegisterExchange(
		[FromBody] RegisterExchangeRequest request,
		CancellationToken ct)
	{
		RegisterExchangeCommand cmd = request.ToCommand();
		CommandResult<ActivityId> result = await mediator.Send(cmd, ct);

		if (result.Id.IsEmpty)
		{
			throw new InvalidOperationException("CommandResult.Id was null for successful RegisterExchange.");
		}

		return CreatedAtAction(
			actionName: nameof(GetById),
			routeValues: new { id = result.Id.Value },
			value: result);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(ActivityDetails), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ActivityDetails>> GetById([FromRoute] Guid id, CancellationToken ct)
	{
		var activityId = ActivityId.From(id);

		// Prefer: handler throws NotFoundException (then controller stays thin)
		// So the handler should return ActivityDetails (non-null) or throw.
		ActivityDetails details = await mediator.Send(new GetActivityDetailsQuery(activityId), ct);

		return Ok(details);
	}

	[HttpGet]
	[ProducesResponseType(typeof(PagedResult<ActivityListItem>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<PagedResult<ActivityListItem>>> GetList(
		[FromQuery] GetActivityListRequest request,
		CancellationToken ct)
	{
		var query = new GetActivityListQuery(
			AccountId: AccountId.From(request.AccountId),
			PageRequest: new PageRequest(request.PageNumber, request.PageSize),
			FromDate: request.FromDate,
			ToDate: request.ToDate,
			AssetCode: request.AssetCode);

		var result = await mediator.Send(query, ct);
		return Ok(result);
	}
}
