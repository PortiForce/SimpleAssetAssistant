namespace Portiforce.SAA.Core.Identity.Models.Client;

public sealed record TenantRetentionSettings
{
	public int DeletedDataRetentionDays { get; init; } = 30;
}
