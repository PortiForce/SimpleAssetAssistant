using System.Net;

namespace Portiforce.SAA.Contracts.Exceptions;

public class PortiforceApiException : Exception
{
	public PortiforceApiException(
		HttpStatusCode statusCode,
		string message,
		ApiProblemDetails? problemDetails = null,
		Exception? innerException = null)
		: base(message, innerException)
	{
		this.StatusCode = statusCode;
		this.ProblemDetails = problemDetails;
	}

	public HttpStatusCode StatusCode { get; }

	public ApiProblemDetails? ProblemDetails { get; }

	public string? Title => this.ProblemDetails?.Title;

	public string? Detail => this.ProblemDetails?.Detail;

	public string? Type => this.ProblemDetails?.Type;

	public string? Instance => this.ProblemDetails?.Instance;
}