using System.Net.Http.Json;
using System.Security.Claims;

using global::Portiforce.SAA.Contracts.Configuration;
using global::Portiforce.SAA.Web.Client.Configuration;

using Microsoft.AspNetCore.Components.Authorization;

namespace Portiforce.SAA.Web.Client.Services.Security;

public sealed class BffAuthenticationStateProvider(IHttpClientFactory httpClientFactory) : AuthenticationStateProvider
{
	private static readonly AuthenticationState Anonymous =
		new(new ClaimsPrincipal(new ClaimsIdentity()));

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			var http = httpClientFactory.CreateClient(WebClientConstants.NoAntiforgeryClientName);

			var me = await http.GetFromJsonAsync<MeResponse>(ApiRoutes.Ptofile);
			if (me?.IsAuthenticated != true)
			{
				return Anonymous;
			}

			var claims = new List<Claim>();

			if (!string.IsNullOrWhiteSpace(me.UserId))
			{
				claims.Add(new Claim(ClaimTypes.NameIdentifier, me.UserId));
			}

			if (!string.IsNullOrWhiteSpace(me.Email))
			{
				claims.Add(new Claim(ClaimTypes.Email, me.Email));
				claims.Add(new Claim(ClaimTypes.Name, me.Email));
			}

			foreach (string role in me.Roles ?? [])
			{
				if (!string.IsNullOrWhiteSpace(role))
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}
			}

			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "BffCookie")));
		}
		catch
		{
			// If the BFF endpoint isn't reachable (or not implemented yet), treat as anonymous.
			return Anonymous;
		}
	}

	private sealed record MeResponse(bool IsAuthenticated, string? UserId, string? Email, string[]? Roles);
}
