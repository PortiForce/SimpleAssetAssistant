namespace Portiforce.SAA.Core.Identity.Enums;

/// <summary>
///     Describes how the invitation is intended to be delivered and/or accepted.
///     It is a flow/channel concept, not the actual identifier value type.
/// </summary>
public enum InviteChannel : byte
{
	Email = 1,

	Telegram = 2,

	AppleAccount = 3
}