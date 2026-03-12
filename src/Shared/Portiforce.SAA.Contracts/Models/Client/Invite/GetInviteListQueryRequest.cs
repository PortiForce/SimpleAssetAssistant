using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite;

public sealed record GetInviteListQueryRequest(
	string? Search,
	InviteStatus? Status,
	InviteChannel? Channel,
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

		if (Status is not null)
		{
			yield return new KeyValuePair<string, string>("status", Status.ToString()!);
		}

		if (Channel is not null)
		{
			yield return new KeyValuePair<string, string>("channel", Channel.ToString()!);
		}

		yield return new KeyValuePair<string, string>("page", Page.ToString());
		yield return new KeyValuePair<string, string>("pageSize", PageSize.ToString());
	}
}