using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Actions.Commands;
using Portiforce.SimpleAssetAssistant.Application.UseCases.Client.Tenant.Actions.Queries;
using Portiforce.SimpleAssetAssistant.Core.Primitives.Ids;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Contracts.Client.Tenant.Requests;
using Portiforce.SimpleAssetAssistant.Presentation.WebApi.Interfaces;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Tenant;


[Route("api/v1/tenants/{tenantId}")] 
[ApiController]
[Authorize(Policy = "RequireTenantAdmin")]
public sealed class ManageController(
	ITenantIdServiceResolver tenantIdResolver,
	IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetDetails([FromRoute] Guid tenantId, CancellationToken ct)
	{
		TenantId? resolvedTenantId = tenantIdResolver.GetTenantFromHeader(out var problem);
		if (problem is not null)
		{
			return BadRequest(problem);
		}

		if (resolvedTenantId?.Value != tenantId)
		{
			return BadRequest("Tenant ID mismatch");
		}

		var result = await mediator.Send(new GetTenantDetailsQuery(resolvedTenantId.Value), ct);
		return Ok(result);
	}

	[HttpPut]
	public async Task<IActionResult> UpdateSettings(
		[FromRoute] Guid tenantId,
		[FromBody] UpdateTenantRequest request,
		CancellationToken ct)
	{
		TenantId? resolvedTenantId = tenantIdResolver.GetTenantFromHeader(out var problem);
		if (problem is not null)
		{
			return BadRequest(problem);
		}

		if (resolvedTenantId?.Value != tenantId)
		{
			return BadRequest("Tenant ID mismatch");
		}

		var command = new UpdateTenantSettingsCommand(resolvedTenantId.Value, request.DefaultCurrency, request.EnforceTwoFactor);
		await mediator.Send(command, ct);
		return NoContent();
	}

	[HttpGet("stats")]
	public async Task<IActionResult> GetStats([FromRoute] Guid tenantId, CancellationToken ct)
	{
		// Implementation for onboarding stats...
		throw new NotImplementedException();
	}
}
