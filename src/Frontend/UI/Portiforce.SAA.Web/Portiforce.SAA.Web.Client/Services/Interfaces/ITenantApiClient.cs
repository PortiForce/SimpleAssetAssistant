using Portiforce.SAA.Contracts.Models.Client;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface ITenantApiClient
{
	Task<TenantInfo?> GetTenantAsync();
}