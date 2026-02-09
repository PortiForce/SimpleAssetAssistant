namespace Portiforce.SAA.Api.Contracts.Client.Tenant.Requests;

public sealed record UpdateTenantRequest(string BrandName, string DefaultCurrency, bool EnforceTwoFactor);
