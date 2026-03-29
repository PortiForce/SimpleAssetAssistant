using System.Text.Json.Serialization;

namespace Portiforce.SAA.Contracts.Exceptions;

public sealed class ApiValidationProblemDetails : ApiProblemDetails
{
	[JsonPropertyName("errors")] public Dictionary<string, string[]> Errors { get; init; } = [];
}