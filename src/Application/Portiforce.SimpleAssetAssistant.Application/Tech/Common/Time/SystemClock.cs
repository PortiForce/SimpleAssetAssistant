using Portiforce.SimpleAssetAssistant.Application.Interfaces.Common.Time;

namespace Portiforce.SimpleAssetAssistant.Application.Tech.Common.Time;

public sealed class SystemClock : IClock
{
	public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
