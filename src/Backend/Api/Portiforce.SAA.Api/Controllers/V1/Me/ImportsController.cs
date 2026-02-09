using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Core.Activities.Enums;

namespace Portiforce.SAA.Api.Controllers.V1.Me;

[ApiController]
[Route("api/v1/me/{controller}")]
[Authorize]
public sealed class ImportsController(IMediator mediator) : ControllerBase
{
	[HttpPost]
	[Consumes("multipart/form-data")]
	public Task<IActionResult> CreateImportJob(
		[FromServices] ICurrentUser currentUser,
		[FromForm] IFormFile file,
		[FromForm] AssetActivityKind? activityKind)
	{
		return Task.FromResult<IActionResult>(
			StatusCode(StatusCodes.Status501NotImplemented, "Imports are not implemented yet."));
	}

	[HttpGet("{importId:guid}")]
	public Task<IActionResult> GetImportById(
		[FromServices] ICurrentUser currentUser,
		[FromRoute] Guid importId,
		CancellationToken ct)
	{
		return Task.FromResult<IActionResult>(
			StatusCode(StatusCodes.Status501NotImplemented, "Import job details is not implemented yet."));
	}
}
