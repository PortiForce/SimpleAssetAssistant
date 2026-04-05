using System.Text.Json.Serialization;

namespace Portiforce.SAA.Contracts.Exceptions;

public sealed class ApiValidationProblemDetails : ApiProblemDetails
{
	[JsonPropertyName("errors")] public Dictionary<string, string> Errors { get; init; } = [];

	public static ApiValidationProblemDetails CreateProblem(
		int status,
		string title,
		string detail,
		Dictionary<string, string> errors) =>
		new() { Status = status, Title = title, Detail = detail, Errors = errors };
}