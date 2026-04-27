namespace Portiforce.SAA.Application.Enums;

public enum InboxMessageState : byte
{
	Received = 0,

	Processing = 1,

	Processed = 2,

	Failed = 3,

	Dead = 4
}
