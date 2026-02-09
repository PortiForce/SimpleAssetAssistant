using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Api.Contracts.Profile.Account.Requests;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Profile.Account.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Profile.Account.Projections;
using Portiforce.SAA.Core.Primitives;

namespace Portiforce.SAA.Api.Controllers.V1.Me;

[Route("api/v1/me/{controller}")]
[ApiController]
[Authorize]
public sealed class ProfileController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(typeof(CurrentUserDetails), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetMyProfile(
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		if (currentUser.TenantId.IsEmpty || currentUser.Id.IsEmpty)
		{
			return Unauthorized();
		}

		var result = await mediator.Send(new GetAccountDetailsQuery(currentUser.Id, currentUser.TenantId), ct);
		return Ok(result);
	}

	[HttpPatch]
	public async Task<IActionResult> UpdateMyProfile(
		[FromServices] ICurrentUser currentUser,
		[FromBody] UpdateProfileRequest request,
		CancellationToken ct)
	{
		if (currentUser.TenantId.IsEmpty || currentUser.Id.IsEmpty)
		{
			return Unauthorized();
		}

		PhoneNumber? phoneNumber = null;
		Email? backupEmail = null;

		if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
		{
			if (!PhoneNumber.TryCreate(request.PhoneNumber, out var ph))
			{
				return BadRequest("Invalid phone number format.");
			}
			phoneNumber = ph;
		}

		if (!string.IsNullOrWhiteSpace(request.BackupEmail))
		{
			if (!Email.TryCreate(request.BackupEmail, out var em))
			{
				return BadRequest("Invalid email format.");
			}
			backupEmail = em;
		}

		var command = new UpdateProfileCommand(
			currentUser.Id,
			request.Alias,
			phoneNumber,
			backupEmail,
			request.Locale,
			request.DefaultCurrency);

		await mediator.Send(command, ct);
		return NoContent();
	}

	[HttpPut("strategy")]
	public Task<IActionResult> UpdateStrategy(
		[FromServices] ICurrentUser currentUser,
		[FromBody] UpdateStrategyRequest request,
		CancellationToken ct)
	{
		return Task.FromResult<IActionResult>(
			StatusCode(StatusCodes.Status501NotImplemented, "Profile strategy is not implemented yet."));
	}
}
