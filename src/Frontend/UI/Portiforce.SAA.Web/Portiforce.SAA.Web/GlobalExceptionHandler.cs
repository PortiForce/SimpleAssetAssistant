using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Portiforce.SAA.Web;

public sealed class GlobalExceptionHandler(
	ILogger<GlobalExceptionHandler> logger,
	IProblemDetailsService problemDetailsService) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		var traceId = httpContext.TraceIdentifier;

		var (status, title, type) = exception switch
		{
			SqlException => (
				StatusCodes.Status503ServiceUnavailable,
				"Database is temporarily unavailable.",
				"https://httpstatuses.com/503"),

			TimeoutException => (
				StatusCodes.Status503ServiceUnavailable,
				"Operation timed out.",
				"https://httpstatuses.com/503"),

			OperationCanceledException when httpContext.RequestAborted.IsCancellationRequested => (
				499, // client closed request, optional internal convention
				"Request was cancelled by the client.",
				"about:blank"),

			_ => (
				StatusCodes.Status500InternalServerError,
				"An unexpected server error occurred.",
				"https://httpstatuses.com/500")
		};

		logger.LogError(exception,
			"Unhandled exception. TraceId: {TraceId}, Path: {Path}",
			traceId,
			httpContext.Request.Path);

		httpContext.Response.StatusCode = status;

		await problemDetailsService.WriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			ProblemDetails = new ProblemDetails
			{
				Status = status,
				Title = title,
				Type = type,
				Detail = httpContext.RequestServices
					.GetRequiredService<IHostEnvironment>()
					.IsDevelopment()
						? exception.Message
						: null,
				Extensions = { ["traceId"] = traceId }
			},
			Exception = exception
		});

		return true;
	}
}
