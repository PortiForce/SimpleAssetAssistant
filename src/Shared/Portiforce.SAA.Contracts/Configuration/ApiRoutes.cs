namespace Portiforce.SAA.Contracts.Configuration;

public static class ApiRoutes
{
	public const string Public = "/public";

	public const string Auth = "/auth";

	public const string BffRoot = "/bff";

	public const string Profile = "/bff/me";

	public const string Tenant = "/bff/tenant";

	public const string Admin = "/bff/admin";

	public const string Platform = "/bff/platform";

	public static class AdminInviteRoutes
	{
		public const string Root = $"/{BffRoot}/admin/invites";

		public const string Summary = $"{Root}/summary";

		public const string New = $"{Root}/new";

		public static string Details(Guid inviteId) => $"{Root}/{inviteId}";

		public static string InviteResend(Guid inviteId) => $"{Root}/{inviteId}/resend";

		public static string InviteRevoke(Guid inviteId) => $"{Root}/{inviteId}/revoke";
	}

	public static class PublicInviteRoutes
	{
		public const string Root = $"/{BffRoot}/invite";

		public static string OverviewInvite(string inviteToken) => $"/{Root}/{inviteToken}";

		public static string DeclineInvite(string inviteToken) => $"/{Root}/{inviteToken}/decline";

		public static string InitAcceptInvite(string inviteToken) => $"/{Root} /{inviteToken}/accept";
	}

	public static class Accounts
	{
		public const string Root = $"/{BffRoot}/accouts";

		public static string Details(Guid inviteId) => $"{Root}/{inviteId}";
	}
}