namespace Portiforce.SAA.Application.Interfaces.Common.Time;

public interface IClock
{
	DateTimeOffset UtcNow { get; }
}