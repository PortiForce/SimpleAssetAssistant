using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Auth.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Auth.Result;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Core.Identity;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Configuration;
using Portiforce.SAA.Web.Infrastructure;

namespace Portiforce.SAA.Web.Features.Endpoints;

public sealed class AuthEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup(ApiRoutes.Auth).WithTags("Authentication");

		group.MapPost("/login", LoginAsync)
			.WithName("LocalLogin");

		group.MapGet("/login/google", TriggerGoogleLogin)
			.WithName("GoogleLogin");

		group.MapGet("/google-callback", HandleGoogleCallbackAsync)
			.WithName("GoogleCallback");

		group.MapGet("/access-denied", () => Results.Text("Access denied."));

		group.MapPost("/logout", LogoutAsync)
			.WithName("LogoutPost");
	}

	private static async Task<IResult> LoginAsync(
			[FromBody] LoginRequest request,
			[FromServices] IMediator mediator,
			[FromServices] ITenantContext tenantContext,
			HttpContext context,
			CancellationToken ct)
	{
		Guid? tenantId = tenantContext.TenantId;
		if (tenantId == null || tenantId == Guid.Empty)
		{
			return TypedResults.Redirect("/auth/access-denied?reason=tenant_context_lost");
		}

		return TypedResults.Problem(
			title: "Local login is disabled",
			detail: "Use the Sign in page and choose “Continue with Google”.",
			statusCode: StatusCodes.Status501NotImplemented);
	}

	private static IResult TriggerGoogleLogin(
		[FromServices] ITenantContext tenantContext,
		[FromQuery] string? inviteToken)
	{
		if (!tenantContext.TenantId.HasValue || tenantContext.TenantId.Value == Guid.Empty)
		{
			return TypedResults.Redirect("/auth/access-denied?reason=tenant_context_lost");
		}

		var properties = new AuthenticationProperties
		{
			RedirectUri = "/auth/google-callback"
		};

		properties.Items[WebConstants.TenantIdName] = tenantContext.TenantId.Value.ToString();

		if (!string.IsNullOrWhiteSpace(inviteToken))
		{
			properties.Items[WebConstants.InviteTokenName] = inviteToken;
		}

		return TypedResults.Challenge(
			properties,
			new[] { GoogleDefaults.AuthenticationScheme });
	}

	private static async Task<IResult> HandleGoogleCallbackAsync(
	HttpContext context,
	[FromServices] IMediator mediator,
	CancellationToken ct)
	{
		AuthenticateResult authenticateResult =
			await context.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

		if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
		{
			return TypedResults.Redirect("/auth/access-denied?reason=google_auth_failed");
		}

		Guid? rawTenantId = null;
		string? inviteTokenStr = null;

		if (authenticateResult.Properties?.Items is { } items)
		{
			if (items.TryGetValue(WebConstants.TenantIdName, out string? tenantIdStr) &&
				Guid.TryParse(tenantIdStr, out Guid parsedTenantId))
			{
				rawTenantId = parsedTenantId;
			}

			if (items.TryGetValue(WebConstants.InviteTokenName, out string? token))
			{
				inviteTokenStr = token;
			}
		}

		if (rawTenantId is null || rawTenantId == Guid.Empty)
		{
			return TypedResults.Redirect("/auth/access-denied?reason=tenant_context_lost");
		}

		string? email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
		string? subjectId = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
		string firstName = authenticateResult.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
		string lastName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(subjectId))
		{
			return TypedResults.Redirect("/auth/access-denied?reason=incomplete_profile");
		}

		TenantId tenantId = new(rawTenantId.Value);

		AccountId accountId;
		Role role;
		AccountState state;

		if (!string.IsNullOrWhiteSpace(inviteTokenStr))
		{
			var command = new AcceptInviteWithGoogleCommand(
				tenantId,
				inviteTokenStr,
				email,
				subjectId,
				firstName,
				lastName);

			TypedResult<AcceptInviteResult> result = await mediator.Send(command, ct);

			if (!result.IsSuccess)
			{
				string error = Uri.EscapeDataString(result.Error?.Code ?? "unknown");
				return TypedResults.Redirect($"/auth/invite-accept-failed?error={error}");
			}

			accountId = result.Value.AccountId;
			role = result.Value.Role;
			state = result.Value.State;
		}
		else
		{
			var command = new LoginWithGoogleExternalIdCommand(
				tenantId,
				subjectId,
				firstName,
				lastName);

			TypedResult<LoginWithGoogleResult> result = await mediator.Send(command, ct);

			if (!result.IsSuccess)
			{
				string error = Uri.EscapeDataString(result.Error?.Code ?? "unknown");
				return TypedResults.Redirect($"/auth/access-denied?error={error}");
			}

			accountId = result.Value.AccountId;
			role = result.Value.Role;
			state = result.Value.State;
		}

		var claims = new List<Claim>
	{
		new(ClaimTypes.NameIdentifier, accountId.ToString()),
		new(ClaimTypes.Name, $"{firstName} {lastName}".Trim()),
		new(ClaimTypes.Email, email),
		new(ClaimTypes.Role, role.ToString()),
		new(CustomClaimTypes.State, state.ToString()),
		new(CustomClaimTypes.TenantId, tenantId.ToString())
#if DEBUG
        ,new("GoogleSub", subjectId)
#endif
    };

		var identity = new ClaimsIdentity(
			claims,
			CookieAuthenticationDefaults.AuthenticationScheme);

		await context.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			new ClaimsPrincipal(identity),
			new AuthenticationProperties
			{
				IsPersistent = false,
				ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
			});

		return TypedResults.Redirect("/");
	}

	private static async Task<IResult> LogoutAsync(
		HttpContext context,
		[FromForm] string returnUrl)
	{
		await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return TypedResults.LocalRedirect("/");
	}
}