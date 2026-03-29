using System.Globalization;

using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite.Summary;

public sealed record GetInviteSummaryRequest(
	HashSet<InviteStatus>? Statuses,
	HashSet<InviteChannel>? Channels,
	bool? HasAccount = null,
	bool? IncludeRevoked = null,
	DateTime? FromDate = null,
	DateTime? ToDate = null,
	InviteSummaryRange Range = InviteSummaryRange.Today,
	InviteTrendBucket Trend = InviteTrendBucket.Hour)
{
	public IEnumerable<KeyValuePair<string, string>> ToQueryParameters()
	{
		yield return new KeyValuePair<string, string>("range", this.Range.ToString());

		yield return new KeyValuePair<string, string>("trendBucket", this.Trend.ToString());

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

		if (this.FromDate is not null)
		{
			yield return new KeyValuePair<string, string>(
				"fromDate",
				this.FromDate.Value.ToString(CultureInfo.InvariantCulture));
		}

		if (this.ToDate is not null)
		{
			yield return new KeyValuePair<string, string>(
				"toDate",
				this.ToDate.Value.ToString(CultureInfo.InvariantCulture));
		}

		if (this.HasAccount is not null)
		{
			yield return new KeyValuePair<string, string>("hasAccount", this.HasAccount.Value.ToString());
		}

		if (this.IncludeRevoked is not null)
		{
			yield return new KeyValuePair<string, string>("includeRevoked", this.IncludeRevoked.Value.ToString());
		}
	}
}