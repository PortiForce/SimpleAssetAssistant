using System.Text.Json;

using Portiforce.SAA.Application.Tech.Abstractions.Serialization;

namespace Portiforce.SAA.Infrastructure.Serialization;

internal sealed class SystemTextJsonSerializer : IJsonSerializer
{
	private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

	public string Serialize<T>(T value) =>
		JsonSerializer.Serialize(value, Options);

	public T Deserialize<T>(string json) =>
		JsonSerializer.Deserialize<T>(json, Options)
		?? throw new InvalidOperationException("JSON payload could not be deserialized.");

	public object? Deserialize(string json, Type returnType) =>
		JsonSerializer.Deserialize(json, returnType, Options);
}
