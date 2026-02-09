using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portiforce.SAA.Api.Controllers.V1.Me;

[Route("api/v1/me/{controller}")]
[ApiController]
[Authorize]
public sealed class WatchListsController : ControllerBase
{
	// todo:
	// 1. get watchlists overview, check watchlists health and configuration
	// 2. update watchlists : set priority assets, prices to accumulate 
}