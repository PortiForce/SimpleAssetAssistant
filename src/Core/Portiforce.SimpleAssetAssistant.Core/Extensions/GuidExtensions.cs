namespace Portiforce.SimpleAssetAssistant.Core.Extensions;

/// <summary>
/// Provides utility methods for creating, parsing, and formatting globally unique identifiers (GUIDs) for use as IDs.
/// </summary>
/// <remarks>This class offers convenience methods for working with GUIDs, including generating version 7 GUIDs,
/// parsing string representations, and formatting GUIDs in a standard display format. All members are static and thread
/// safe.</remarks>
public static class GuidExtensions
{
	public static Guid New() => Guid.CreateVersion7();
	public static bool TryParse(string? raw, out Guid value) => Guid.TryParse(raw, out value);
	public static string ToString(Guid value) => value.ToString("D");
}
