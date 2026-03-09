using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Account;

public sealed record GetAccountListQueryRequest(
	string? Search,
	UiAccountState? State,
	UiAccountTier? Tier,
	int Page = 1,
	int PageSize = 20)
{
	public IEnumerable<KeyValuePair<string, string>> ToQueryParameters()
	{
		if (!string.IsNullOrWhiteSpace(Search))
		{
			yield return new KeyValuePair<string, string>("search", Search);
		}

		if (State is not null)
		{
			yield return new KeyValuePair<string, string>("state", State.ToString()!);
		}

		if (Tier is not null)
		{
			yield return new KeyValuePair<string, string>("tier", Tier.ToString()!);
		}

		yield return new KeyValuePair<string, string>("page", Page.ToString());
		yield return new KeyValuePair<string, string>("pageSize", PageSize.ToString());
	}
}