using System.Net.Http.Json;

using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services;

public sealed class TenantApiClient(HttpClient http) : ITenantApiClient
{
    public Task<TenantInfo?> GetTenantAsync()
        => http.GetFromJsonAsync<TenantInfo>(ApiRoutes.BffRoot);
}