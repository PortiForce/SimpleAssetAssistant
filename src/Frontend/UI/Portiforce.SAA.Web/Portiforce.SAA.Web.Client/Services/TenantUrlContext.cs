using Microsoft.AspNetCore.Components;

using Portiforce.SAA.Contracts.Contexts;

namespace Portiforce.SAA.Web.Client.Services;

public sealed class TenantUrlContext(NavigationManager navigationManager) : ITenantUrlContext
{
	public bool IsTenantResolved()
	{
		string[] parts = GetDomainParts();
		return parts.Length > 2;
	}

	public string[] GetDomainParts()
	{
		var uri = new Uri(navigationManager.Uri);
		return uri.Host.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	}
}
