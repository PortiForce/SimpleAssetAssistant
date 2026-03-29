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
		if (!string.IsNullOrWhiteSpace(this.Search))
		{
			yield return new KeyValuePair<string, string>("search", this.Search);
		}

		if (this.Statuses is not null && this.Statuses.Count != 0)
		{
			foreach (InviteStatus status in this.Statuses)
			{
				yield return new KeyValuePair<string, string>("statuses", status.ToString());
			}
		}

		if (this.Channels is not null && this.Channels.Count != 0)
		{
			foreach (InviteChannel channel in this.Channels)
			{
				yield return new KeyValuePair<string, string>("channels", channel.ToString());
			}
		}

		if (this.HasAccount is not null)
		{
			yield return new KeyValuePair<string, string>("hasAccount", this.HasAccount.Value.ToString());
		}

		yield return new KeyValuePair<string, string>("page", this.Page.ToString());
		yield return new KeyValuePair<string, string>("pageSize", this.PageSize.ToString());
	}
}