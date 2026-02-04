using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;
using Portiforce.SimpleAssetAssistant.Core.Activities.Enums;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Me;

[ApiController]
[Route("api/v1/me/[controller]")]
[Authorize]
public sealed class ImportsController(IMediator mediator) : ControllerBase
{
	[HttpPost("createimportjob")]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> CreateImportJob(IFormFile file, AssetActivityKind? activityKind)
	{
		throw new NotImplementedException("Imports is not yet supported");
	}
}
