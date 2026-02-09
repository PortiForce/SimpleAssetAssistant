using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portiforce.SAA.Application.Exceptions;

namespace Portiforce.SAA.Api.ErrorHandling;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
	/// <summary>
	/// todo tech: review and check approach
	/// </summary>
	/// <param name="httpContext"></param>
	/// <param name="exception"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		// If response already started, let the default handler deal with it
		if (httpContext.Response.HasStarted)
		{
			return false;
		}

		var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

		// IMPORTANT: don’t leak sensitive info to clients (keep details minimal)
		// Microsoft guidance warns against returning sensitive error details.

		var (statusCode, title, type, extensions) = exception switch
		{
			BadRequestException ex => (
				StatusCodes.Status400BadRequest,
				"Bad Request",
				"https://httpstatuses.com/400",
				new Dictionary<string, object?> { ["message"] = ex.Message }),

			ApplicationValidationException ex => (
				StatusCodes.Status400BadRequest,
				"Validation Error",
				"https://httpstatuses.com/400",
				new Dictionary<string, object?> { ["errors"] = ex.Errors }),

			NotFoundException ex => (
				StatusCodes.Status404NotFound,
				"Not Found",
				"https://httpstatuses.com/404",
				new Dictionary<string, object?> { ["message"] = ex.Message }),

			ConflictException ex => (
				StatusCodes.Status409Conflict,
				"Conflict",
				"https://httpstatuses.com/409",
				new Dictionary<string, object?> { ["message"] = ex.Message }),

			ForbiddenException ex => (
				StatusCodes.Status403Forbidden,
				"Forbidden",
				"https://httpstatuses.com/403",
				new Dictionary<string, object?> { ["message"] = ex.Message }),

			_ => (
				StatusCodes.Status500InternalServerError,
				"Internal Server Error",
				"https://httpstatuses.com/500",
				new Dictionary<string, object?> { ["message"] = "Unexpected error." })
		};

		if (statusCode >= 500)
		{
			logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);
		}

		var problem = new ProblemDetails
		{
			Status = statusCode,
			Title = title,
			Type = type,
			Instance = httpContext.Request.Path,
			Extensions =
			{
				["traceId"] = traceId
			}
		};

		foreach (var kv in extensions)
		{
			problem.Extensions[kv.Key] = kv.Value;
		}

		httpContext.Response.StatusCode = statusCode;
		await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

		return true;
	}
}
