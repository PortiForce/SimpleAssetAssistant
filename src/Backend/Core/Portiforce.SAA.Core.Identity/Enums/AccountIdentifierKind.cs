namespace Portiforce.SAA.Core.Identity.Enums;

/// <summary>
///     Describes which tenant-scoped identifiers are reserved/owned by an account.
///     It is an account ownership / uniqueness taxonomy.
/// </summary>
public enum AccountIdentifierKind : byte
{
	None = 0,

	Email = 1,

	Phone = 2,

	TelegramUserName = 3,

	TelegramUserId = 4
}