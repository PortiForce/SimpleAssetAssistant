namespace Portiforce.SAA.Core.Identity.Enums;

public enum TokenRevokeReason : byte
{
	/// <summary>
	/// User explicitly clicked "Logout"
	/// Action: None. Normal behavior.
	/// </summary>
	UserLogout = 1,

	/// <summary>
	/// The token was used to get a new pair (Rotation)
	/// Action: None. This is the happy path for refresh tokens.
	/// </summary>
	ReplacedByNewToken = 2,

	/// <summary>
	/// Admin or System manually revoked it (e.g. banning a user)
	/// Action: Force logout on client.
	/// </summary>
	AdministrativeRevocation = 3,

	/// <summary>
	/// User changed password or enabled 2FA
	/// Action: Revoke all existing sessions to force re-login.
	/// </summary>
	SecurityPolicyChange = 4,

	/// <summary>
	/// Detection of theft (e.g. attempting to use an already-revoked token)
	///  Action: CRITICAL. Revoke ALL tokens for this user and alert them.
	/// </summary>
	TheftDetected = 5
}
