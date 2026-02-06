using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SimpleAssetAssistant.Application.Tech.Messaging;

namespace Portiforce.SimpleAssetAssistant.Presentation.WebApi.Controllers.V1.Me;

[ApiController]
[Route("api/v1/me/{controller}")]
[Authorize]
public sealed class PortfoliosController(IMediator mediator) : ControllerBase
{
	// todo:
	// 1. get portfolios overview, check portfolio health and configuration
	// 2. update portfrolios : set priority assets, prices to accumulate 
}
