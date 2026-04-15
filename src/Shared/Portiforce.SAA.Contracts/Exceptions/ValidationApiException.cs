using System.Net;

namespace Portiforce.SAA.Contracts.Exceptions;

public sealed class ValidationApiException : PortiforceApiException
{
	public ValidationApiException(
		HttpStatusCode statusCode,
		string message,
		ApiValidationProblemDetails problemDetails)
		: base(statusCode, message, problemDetails)
	{
		this.Errors = problemDetails.Errors;
	}

	public IReadOnlyDictionary<string, string> Errors { get; }
}