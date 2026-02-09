using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Api.Contracts.Activity.Mappers;
using Portiforce.SAA.Api.Contracts.Activity.Requests.Activity;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Activity.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Activity.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Activity.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Api.Controllers.V1.Me;

[ApiController]
[Route("api/v1/me/{controller}")]
[Authorize]
public sealed class ActivitiesController(IMediator mediator) : ControllerBase
{
	// todo:
	// 1. add other atomic actions: transfer, income, adjustment, etc

	[HttpPost("trades")]
	[ProducesResponseType(typeof(CommandResult<ActivityId>), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	public async Task<ActionResult<CommandResult<ActivityId>>> RegisterTrade(
		[FromServices] ICurrentUser currentUser,
		[FromBody] RegisterTradeRequest request,
		CancellationToken ct)
	{
		if (currentUser.TenantId.IsEmpty || currentUser.Id.IsEmpty)
		{
			return Unauthorized();
		}

		RegisterTradeCommand cmd = request.ToCommand(currentUser.Id, currentUser.TenantId);
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
		[FromServices] ICurrentUser currentUser,
		[FromBody] RegisterExchangeRequest request,
		CancellationToken ct)
	{
		if (currentUser.TenantId.IsEmpty || currentUser.Id.IsEmpty)
		{
			return Unauthorized();
		}

		RegisterExchangeCommand cmd = request.ToCommand(currentUser.Id, currentUser.TenantId);
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
	public async Task<ActionResult<ActivityDetails>> GetById(
		[FromServices] ICurrentUser currentUser,
		[FromRoute] Guid id,
		CancellationToken ct)
	{
		if (currentUser.TenantId.IsEmpty || currentUser.Id.IsEmpty)
		{
			return Unauthorized();
		}

		var activityId = ActivityId.From(id);

		ActivityDetails? details = await mediator.Send(
			new GetActivityDetailsQuery(
				activityId,
				currentUser.TenantId,
				currentUser.Id),
			ct);

		if (details is null)
		{
			return NotFound();
		}

		return Ok(details);
	}

	[HttpGet]
	[ProducesResponseType(typeof(PagedResult<ActivityListItem>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<PagedResult<ActivityListItem>>> GetList(
		[FromServices] ICurrentUser currentUser,
		[FromQuery] GetActivityListRequest request,
		CancellationToken ct)
	{
		if (currentUser.TenantId.IsEmpty || currentUser.Id.IsEmpty)
		{
			return Unauthorized();
		}

		var query = new GetActivityListQuery(
			AccountId: currentUser.Id,
			TenantId: currentUser.TenantId,
			PageRequest: new PageRequest(request.PageNumber, request.PageSize),
			FromDate: request.FromDate,
			ToDate: request.ToDate,
			AssetCode: request.AssetCode);

		var result = await mediator.Send(query, ct);
		return Ok(result);
	}
}
