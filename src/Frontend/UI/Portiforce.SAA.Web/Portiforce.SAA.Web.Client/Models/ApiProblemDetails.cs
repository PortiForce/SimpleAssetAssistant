namespace Portiforce.SAA.Web.Client.Models;

/// <summary>
/// custom problem details to keep my WASM decoupled from ASP.NET Core MVC.
/// </summary>
public class ApiProblemDetails
{
	public string? Type { get; set; }
	public string? Title { get; set; }
	public int? Status { get; set; }
	public string? Detail { get; set; }
	public string? Instance { get; set; }

	// Minimal APIs ValidationProblem populates this dictionary
	public Dictionary<string, string[]>? Errors { get; set; }
}
