namespace Portiforce.SAA.Contracts.Enums;

public enum InviteStatus : byte
{
	Unknown = 0,
	Pending = 1,
	Accepted = 2,
	Expired = 3,
	Revoked = 4,
	Declined = 5,
	Failed = 6
}
