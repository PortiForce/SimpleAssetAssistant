namespace Portiforce.SimpleAssetAssistant.Application.Interfaces.Common.Time;

public interface IClock
{
	DateTimeOffset UtcNow { get; }
}