using Portiforce.SimpleAssetAssistant.Application.Interfaces.Common.Time;

namespace Portiforce.SimpleAssetAssistant.Infrastructure.Services.Time;

public sealed class SystemClock : IClock
{
	public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
