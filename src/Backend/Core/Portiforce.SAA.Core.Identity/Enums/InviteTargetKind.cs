namespace Portiforce.SAA.Core.Identity.Enums;

/// <summary>
///     Describes what kind of identifier value the invite actually contains.
///     It is a target-value taxonomy.
/// </summary>
public enum InviteTargetKind : byte
{
	None = 0,

	Email = 1,

	Phone = 2,

	TelegramUserName = 3,

	TelegramUserId = 4
}