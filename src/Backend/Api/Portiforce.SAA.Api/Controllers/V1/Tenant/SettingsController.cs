using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Api.Contracts.Client.Tenant.Requests;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Client.Tenant.Actions.Queries;

namespace Portiforce.SAA.Api.Controllers.V1.Tenant;


[Route("api/v1/tenant/{controller}")] 
[ApiController]
[Authorize(Policy = "RequireTenantAdmin")]
public sealed class SettingsController(
	IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetDetails(
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		var result = await mediator.Send(new GetTenantDetailsQuery(currentUser.TenantId), ct);
		return Ok(result);
	}

	[HttpPut]
	public async Task<IActionResult> UpdateSettings(
		[FromServices] ICurrentUser currentUser,
		[FromBody] UpdateTenantRequest request,
		CancellationToken ct)
	{
		var command = new UpdateTenantSettingsCommand(currentUser.TenantId, request.DefaultCurrency, request.EnforceTwoFactor);
		await mediator.Send(command, ct);
		return NoContent();
	}

	[HttpGet("stats")]
	public Task<IActionResult> GetStats(
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		// Implementation for onboarding stats...
		return Task.FromResult<IActionResult>(
			StatusCode(StatusCodes.Status501NotImplemented, "Tenant statistics summary is not implemented yet."));
	}
}
