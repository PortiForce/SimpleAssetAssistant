using Portiforce.SAA.Application.Interfaces.Common.Time;

namespace Portiforce.SAA.Infrastructure.Services.Time;

public sealed class SystemClock : IClock
{
	public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
