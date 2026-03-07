using Azure;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Models.Auth;
using Portiforce.SAA.Application.Models.Common.DataAccess;
using Portiforce.SAA.Application.Tech.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Invite.Actions.Queries;
using Portiforce.SAA.Application.UseCases.Invite.Projections;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Contracts.Enums;
using Portiforce.SAA.Contracts.Models.Invite;
using Portiforce.SAA.Contracts.UiSetup;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Identity.Models.Invite;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Mappers;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

using InviteChannel = Portiforce.SAA.Contracts.Enums.InviteChannel;

namespace Portiforce.SAA.Web.Features.Endpoints.Tenants;

public sealed class InviteEndpoints : IEndpoint
{
	/*
		GET    /tenant/invites
		GET    /tenant/invites/{inviteId:guid}
		GET    /tenant/invites/template
		POST   /tenant/invites
		POST   /tenant/invites/{inviteId:guid}/resend
		POST   /tenant/invites/{inviteId:guid}/revoke
	 */

	private const int DefaultInviteLifetimeHours = 48;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/tenant/invites")
			.WithTags("Tenant Invites")
			.RequireAuthorization(UiPolicies.TenantAdmin);

		group.MapGet(string.Empty, ListInvitesAsync)
			.WithName("ListTenantInvites")
			.Produces<InviteListResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden);

		group.MapGet("/{inviteId:guid}", GetInviteDetailsAsync)
			.WithName("GetTenantInviteDetails")
			.Produces<InviteDetailsResponse>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound);

		group.MapGet("/template", GetCreateInviteTemplateAsync)
			.WithName("GetCreateInviteTemplate")
			.Produces<CreateInviteRequest>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden);

		group.MapPost(string.Empty, CreateInviteAsync)
			.WithName("CreateTenantInvite")
			.Produces<CreateInviteResponse>(StatusCodes.Status201Created)
			.ProducesValidationProblem()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status409Conflict);

		group.MapPost("/{inviteId:guid}/resend", ResendInviteAsync)
			.WithName("ResendTenantInvite")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);

		group.MapPost("/{inviteId:guid}/revoke", RevokeInviteAsync)
			.WithName("RevokeTenantInvite")
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);
	}

	private static async Task<Results<Ok<InviteListResponse>, UnauthorizedHttpResult, ForbidHttpResult>> ListInvitesAsync(
		[AsParameters] GetInviteListQueryRequest request,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		var guardResult = EnsureTenantAccess<InviteListResponse>(currentUser);
		if (guardResult is not null)
		{
			return guardResult;
		}

		InviteState? status = request.Status?.ToBusiness();
		Core.Identity.Enums.InviteChannel? channel = request.Channel?.ToBusiness();

		var query = new GetInviteListQuery(
			currentUser.TenantId,
			request.Search,
			status,
			channel,
			request.Page,
			request.PageSize);

		PagedResult<InviteListItem> result = await mediator.Send(query, ct);

		InviteListResponse response = result.MapToInviteList();
		return TypedResults.Ok(response);
	}

	private static async Task<Results<Ok<InviteDetailsResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound>> GetInviteDetailsAsync(
		Guid inviteId,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated)
		{
			return TypedResults.Unauthorized();
		}

		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		var getDetailsCommand = new GetInviteDetailsQuery(currentUser.TenantId, inviteId);
		TypedResult<InviteDetails> result = await mediator.Send(getDetailsCommand, ct);

		if (!result.IsSuccess || result.Value == null) 
		{
			return TypedResults.NotFound();
		}

		InviteDetailsResponse inviteDetailsResponse = result.Value.MapToInviteDetails();
		return TypedResults.Ok(inviteDetailsResponse);
	}

	private static Results<Ok<CreateInviteRequest>, UnauthorizedHttpResult, ForbidHttpResult> GetCreateInviteTemplateAsync(
		[FromServices] ICurrentUser currentUser)
	{
		var guardResult = EnsureTenantAccess<CreateInviteRequest>(currentUser);
		if (guardResult is not null)
		{
			return guardResult;
		}

		CreateInviteRequest inviteTemplate = new CreateInviteRequest
		{
			Channel = InviteChannel.Email,
			IntendedRole = InviteTenantRole.TenantUser,
			IntendedTier = InviteAccountTier.Investor,
			TargetValue = string.Empty
		};

		return TypedResults.Ok(inviteTemplate);
	}

	private static async Task<Results<Created<CreateInviteResponse>, ValidationProblem, ProblemHttpResult, UnauthorizedHttpResult, ForbidHttpResult>> CreateInviteAsync(
		[FromBody] CreateInviteRequest request,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		[FromServices] IClock clock,
		HttpContext httpContext,
		CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated)
		{
			return TypedResults.Unauthorized();
		}

		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		Dictionary<string, string[]> errors = Validate(request);
		if (errors.Count > 0)
		{
			return TypedResults.ValidationProblem(errors);
		}

		InviteTarget inviteTarget;
		try
		{
			inviteTarget = request.Channel switch
			{
				InviteChannel.Email => InviteTarget.Email(request.TargetValue),
				InviteChannel.Telegram => InviteTarget.Telegram(request.TargetValue),
				InviteChannel.AppleId => InviteTarget.AppleId(request.TargetValue),
				_ => throw new ArgumentOutOfRangeException(nameof(request.Channel))
			};
		}
		catch (ArgumentException ex)
		{
			return TypedResults.Problem(
				title: "Invalid invite target",
				detail: ex.Message,
				statusCode: StatusCodes.Status400BadRequest);
		}


		Role role = request.IntendedRole.ToBusiness();

		AccountTier tier = request.IntendedTier.ToBusiness();

		if (role == Role.None || tier == AccountTier.None)
		{
			return TypedResults.Problem(
				title: "Invalid invite payload",
				detail: "Unsupported role or tier.",
				statusCode: StatusCodes.Status400BadRequest);
		}

		DateTimeOffset now = clock.UtcNow;
		DateTimeOffset expiresAtUtc = now.AddHours(DefaultInviteLifetimeHours);

		var command = new CreateInviteCommand(
			TenantId: currentUser.TenantId,
			InviteTarget: inviteTarget,
			IntendedRole: role,
			IntendedTier: tier,
			InvitedByAccountId: currentUser.Id,
			CreatedAtUtc: now,
			ExpiredAtUtc: expiresAtUtc);

		TypedResult<CreateInviteResult> result = await mediator.Send(command, ct);

		if (!result.IsSuccess)
		{
			return TypedResults.Problem(
				title: "Invite creation failed",
				detail: result.Error?.Message,
				statusCode: MapToStatusCode(result));
		}

		string acceptInviteUrl = string.Empty;
		string declineInviteUrl = string.Empty;
		if (inviteTarget.Type == Core.Identity.Enums.InviteChannel.Email)
		{
			acceptInviteUrl = BuildAbsoluteUrl(httpContext, $"/auth/login/google?inviteToken={result.Value.Token}");
			declineInviteUrl = BuildAbsoluteUrl(httpContext, $"/publicActions/declineInvite?inviteToken={result.Value.Token}");
		}
		
		var response = new CreateInviteResponse(
			InviteId: result.Value.InviteId,
			AcceptInviteUrl: acceptInviteUrl,
			DeclineInviteUrl: declineInviteUrl,
			ExpiresAtUtc: result.Value.ExpiresAtUtc);

		return TypedResults.Created($"/tenant/invites/{result.Value.InviteId}", response);
	}

	private static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>> ResendInviteAsync(
		Guid inviteId,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated)
		{
			return TypedResults.Unauthorized();
		}

		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		// todo : implement me
		//var resendInviteCommand = new ResendInviteCommand(currentUser.TenantId, inviteId);

		//TypedResult<ResendInviteResult> result = await mediator.Send(resendInviteCommand, ct);

		//if (!result.IsSuccess)
		//{
		//	return TypedResults.Problem(
		//		title: "Invite resend failed",
		//		detail: result.Error?.Message,
		//		statusCode: MapToStatusCode(result));
		//}

		return TypedResults.NoContent();
	}

	private static async Task<Results<NoContent, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>> RevokeInviteAsync(
		Guid inviteId,
		[FromServices] IMediator mediator,
		[FromServices] ICurrentUser currentUser,
		CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated)
		{
			return TypedResults.Unauthorized();
		}

		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		// todo: implement me
		//var revokeInviteCommand = new RevokeInviteCommand(currentUser.TenantId, inviteId);

		//TypedResult<RevokeInviteResult> result = await mediator.Send(revokeInviteCommand, ct);

		//if (!result.IsSuccess)
		//{
		//	return TypedResults.Problem(
		//		title: "Invite resend failed",
		//		detail: result.Error?.Message,
		//		statusCode: MapToStatusCode(result));
		//}

		return TypedResults.NoContent();
	}

	private static Dictionary<string, string[]> Validate(CreateInviteRequest request)
	{
		var errors = new Dictionary<string, string[]>();

		if (string.IsNullOrWhiteSpace(request.TargetValue))
		{
			errors["targetValue"] = ["Target value is required."];
		}

		return errors;
	}

	private static int MapToStatusCode<T>(TypedResult<T> result)
	{
		// todo : adjust to your real error model/codes
		return StatusCodes.Status400BadRequest;
	}

	private static Results<Ok<T>, UnauthorizedHttpResult, ForbidHttpResult>? EnsureTenantAccess<T>(ICurrentUser currentUser)
	{
		if (!currentUser.IsAuthenticated)
		{
			return TypedResults.Unauthorized();
		}

		if (currentUser.TenantId == TenantId.Empty)
		{
			return TypedResults.Forbid();
		}

		return null;
	}

	private static string BuildAbsoluteUrl(HttpContext httpContext, string relativePath)
	{
		// todo : move to invite link builder service
		return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{relativePath}";
	}
}