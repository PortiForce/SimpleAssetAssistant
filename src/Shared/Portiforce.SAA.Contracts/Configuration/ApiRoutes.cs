namespace Portiforce.SAA.Contracts.Configuration;

public static class ApiRoutes
{
	public const string Auth = "/auth";

	public const string BffRoot = "/bff";

	public const string Profile = "/bff/me";

	public const string Tenant = "/bff/tenant";

	public const string Admin = "/bff/admin";

	public const string Platform = "/bff/platform";

	public static class Invites
	{
		public const string Root = "/bff/invites";

		public const string New = "/bff/invites/new";

		public static string Details(Guid inviteId) => $"{Root}/{inviteId}";

		public static string InviteResend(Guid inviteId) => $"{Root}/{inviteId}/resend";

		public static string InviteRevoke(Guid inviteId) => $"{Root}/{inviteId}/revoke";
	}

	public static class Accounts
	{
		public const string Root = "/bff/accouts";

		public static string Details(Guid inviteId) => $"{Root}/{inviteId}";
	}
}
