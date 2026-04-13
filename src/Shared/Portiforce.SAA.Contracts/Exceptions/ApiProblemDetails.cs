using System.Text.Json.Serialization;

namespace Portiforce.SAA.Contracts.Exceptions;

public class ApiProblemDetails
{
	[JsonPropertyName("type")] public string? Type { get; init; }

	[JsonPropertyName("title")] public string? Title { get; init; }

	[JsonPropertyName("status")] public int? Status { get; init; }

	[JsonPropertyName("detail")] public string? Detail { get; init; }

	[JsonPropertyName("instance")] public string? Instance { get; init; }

	/// <summary>
	///     Gets optional extension data like traceId, correlationId, etc.
	/// </summary>
	[JsonExtensionData]
	public Dictionary<string, object?> Extensions { get; init; } = [];

	public static ApiProblemDetails CreateProblem(
		int status,
		string title,
		string detail) =>
		new() { Status = status, Title = title, Detail = detail };
}