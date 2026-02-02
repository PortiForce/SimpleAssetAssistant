using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Models.Common.DataAccess;
using Portiforce.SimpleAssetAssistant.Application.Result;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Actions.Queries;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Profile.Account.Projections;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Profile.Account.Mappers;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Profile.Account.Requests;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Admin;

[ApiController]
[Route("api/v1/tenants/{tenantId}/users")]
[Authorize] // Ensure user is logged in
public class TenantUsersController(
	ITenantIdServiceResolver tenantIdResolver,
	IMediator mediator) : ControllerBase
{
	[HttpPost]
	[Authorize(Policy = "RequireTenantAdmin")]
	[ProducesResponseType(typeof(CommandResult<AccountId>), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> InviteUser(
		[FromBody] InviteUserRequest request,
		CancellationToken ct)
	{
		TenantId? tenantId = tenantIdResolver.GetTenantFromHeader(out var problem);
		if (problem is not null)
		{
			return BadRequest(problem);
		}

		if (tenantId is null)
		{
			return BadRequest("tenant is not defined");
		}

		CreateAccountCommand createAccountCommand = request.ToCommand(tenantId.Value);

		CommandResult<AccountId> result = await mediator.Send(createAccountCommand, ct);

		return CreatedAtAction(
			nameof(GetUserDetails),
			new
			{
				tenantId = tenantId,
				userId = result.Id.Value
			},
			result);
	}

	[HttpGet]
	[Authorize(Policy = "RequireTenantAdmin")]
	[ProducesResponseType(typeof(PagedResult<AccountListItem>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetListOfUsers(
		[FromQuery] PageRequest pageRequest,
		CancellationToken ct)
	{
		TenantId? tenantId = tenantIdResolver.GetTenantFromHeader(out var problem);
		if (problem is not null)
		{
			return BadRequest(problem);
		}

		if (tenantId is null)
		{
			return BadRequest("Tenant header is missing");
		}

		var query = new GetAccountListQuery(tenantId.Value, pageRequest);
		PagedResult<AccountListItem> result = await mediator.Send(query, ct);

		return Ok(result);
	}

	[HttpGet("{userId}")]
	// assume Members can see profiles (e.g. to collaborate).
	[ProducesResponseType(typeof(AccountDetails), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetUserDetails(
		[FromRoute] Guid tenantId, 
		[FromRoute] Guid userId,
		CancellationToken ct)
	{
		// Security Source of Truth
		TenantId? resolvedTenantId = tenantIdResolver.GetTenantFromHeader(out var problem);
		if (problem is not null)
		{
			return BadRequest(problem);
		}

		if (resolvedTenantId?.Value != tenantId)
		{
			return BadRequest("Tenant ID in URL does not match X-Tenant-ID header.");
		}

		// 2. Dispatch Query
		var query = new GetAccountDetailsQuery(
			new AccountId(userId),
			resolvedTenantId.Value);

		AccountDetails result = await mediator.Send(query, ct);

		return Ok(result);
	}
}
