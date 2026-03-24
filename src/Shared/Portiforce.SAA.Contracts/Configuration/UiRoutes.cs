namespace Portiforce.SAA.Contracts.Configuration;

public static class UiRoutes
{
	public static class AdminRoutes
	{
		public static class Invites
		{
			public const string List = "/admin/invites";
			public const string DetailsTemplate = "/admin/invites/{InviteId:guid}";
			public const string New = "/admin/invites/new";

			public static string Details(Guid inviteId) => $"/admin/invites/{inviteId}";

			public static string InviteResend(Guid inviteId) => $"/admin/invites/{inviteId}/resend";

			public static string InviteRevoke(Guid inviteId) => $"/admin/invites/{inviteId}/revoke";
		}

		public static class Accounts
		{
			public const string List = "/admin/accounts";
			public const string DetailsTemplate = "/admin/accounts/{AccountId:guid}";

			public static string Details(Guid accountId) => $"/admin/accounts/{accountId}";
		}
	}
}
