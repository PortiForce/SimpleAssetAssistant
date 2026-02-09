using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Api.Contracts.Client.Tenant.Requests;
using Portiforce.SAA.Api.Contracts.Profile.Account.Mappers;
using Portiforce.SAA.Api.Contracts.Profile.Account.Requests;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Result;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Api.Controllers.V1.Tenant;

[ApiController]
[Route("api/v1/tenant/{controller}")]
[Authorize]
public sealed class UsersController(
	IMediator mediator) : ControllerBase
{
	// todo: 
	// 1 change user's plan and State (only for TenantAdmin)

	[HttpPost]
	[Authorize(Policy = "RequireTenantAdmin")]
	[ProducesResponseType(typeof(CommandResult<AccountId>), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> InviteUser(
		[FromServices] ICurrentUser currentUser,
		[FromBody] InviteUserRequest request,
		CancellationToken ct)
	{
		CreateAccountCommand createAccountCommand = request.ToCommand(currentUser.TenantId);

		CommandResult<AccountId> result = await mediator.Send(createAccountCommand, ct);

		return CreatedAtAction(
			nameof(GetById),
			new
			{
				userId = result.Id.Value
			},
			result);
	}

	[HttpGet]
	[Authorize(Policy = "RequireTenantAdmin")]
	[ProducesResponseType(typeof(PagedResult<AccountListItem>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetList(
		[FromServices] ICurrentUser currentUser,
		[FromQuery] PageRequest pageRequest,
		CancellationToken ct)
	{
		var query = new GetAccountListQuery(currentUser.TenantId, pageRequest);
		PagedResult<AccountListItem> result = await mediator.Send(query, ct);

		return Ok(result);
	}

	[HttpGet("{userId:guid}")]
	[Authorize(Policy = "RequireTenantAdmin")]
	// todo later: assume Members can see profiles (e.g. to collaborate).
	[ProducesResponseType(typeof(AccountDetails), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(
		[FromServices] ICurrentUser currentUser,
		[FromRoute] Guid userId,
		CancellationToken ct)
	{
		var query = new GetAccountDetailsQuery(
			new AccountId(userId),
			currentUser.TenantId);

		AccountDetails result = await mediator.Send(query, ct);

		return Ok(result);
	}

	[HttpPut("{userId:guid}")]
	[Authorize(Policy = "RequireTenantAdmin")]
	public async Task<IActionResult> UpdateUser(
		[FromServices] ICurrentUser currentUser,
		[FromRoute] Guid userId,
		[FromBody] UpdateUserRequest request,
		CancellationToken ct)
	{
		var command = new UpdateAccountCommand(
			currentUser.TenantId,
			new AccountId(userId),
			request.Alias,
			request.Tier,
			request.Role,
			request.State);

		await mediator.Send(command, ct);

		return NoContent();
	}
}
