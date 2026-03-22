using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite;

public sealed record GetInviteListQueryRequest(
	string? Search,
	HashSet<InviteStatus>? Statuses,
	HashSet<InviteChannel>? Channels,
	int Page = 1,
	int PageSize = 20,
	bool? HasAccount = null)
{
	public IEnumerable<KeyValuePair<string, string>> ToQueryParameters()
	{
		if (!string.IsNullOrWhiteSpace(Search))
		{
			yield return new KeyValuePair<string, string>("search", Search);
		}

		if (Statuses is not null && Statuses.Any())
		{
			foreach (var status in Statuses)
			{
				yield return new KeyValuePair<string, string>("statuses", status.ToString());
			}
		}

		if (Channels is not null && Channels.Any())
		{
			foreach (var channel in Channels)
			{
				yield return new KeyValuePair<string, string>("channels", channel.ToString());
			}
		}

		if (HasAccount is not null)
		{
			yield return new KeyValuePair<string, string>("hasAccount", HasAccount.Value.ToString());
		}

		yield return new KeyValuePair<string, string>("page", Page.ToString());
		yield return new KeyValuePair<string, string>("pageSize", PageSize.ToString());
	}
}