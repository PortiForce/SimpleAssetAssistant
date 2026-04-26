namespace Portiforce.SAA.Application.Enums;

public enum OutboxMessageState : byte
{
	Pending = 0,

	Published = 1,

	Processed = 2,

	Failed = 3,

	Dead = 4
}