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
		if (!string.IsNullOrWhiteSpace(this.Search))
		{
			yield return new KeyValuePair<string, string>("search", this.Search);
		}

		if (this.State is not null)
		{
			yield return new KeyValuePair<string, string>("state", this.State.ToString()!);
		}

		if (this.Tier is not null)
		{
			yield return new KeyValuePair<string, string>("tier", this.Tier.ToString()!);
		}

		yield return new KeyValuePair<string, string>("page", this.Page.ToString());

		yield return new KeyValuePair<string, string>("pageSize", this.PageSize.ToString());
	}
}