using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Messaging.Actions.Commands;
using Portiforce.SAA.Application.UseCases.Messaging.Result;
using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Contexts;
using Portiforce.SAA.Contracts.Exceptions;
using Portiforce.SAA.Contracts.Models.Client.Contact;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Web.Infrastructure;
using Portiforce.SAA.Web.Mappers;

namespace Portiforce.SAA.Web.Features.Endpoints;

public sealed class ContactEndpoints : IEndpoint
{
	public const string SendContactMessageEndpointName = "SendContactMessage";

	private const string ContactMessageType = "contact.message.submitted";
	private const string ContactMessageSource = "public-contact-form";

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		_ = app.MapPost(ApiRoutes.Contact, SendMessageAsync)
			.WithName(SendContactMessageEndpointName)
			.WithTags("Public")
			.AllowAnonymous()
			.AddEndpointFilter<ContactMessageDailyRateLimitFilter>()
			.RequireRateLimiting("ContactMessages")
			.Produces<ContactMessageResponse>()
			.ProducesValidationProblem()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status429TooManyRequests);
	}

	private static async Task<Results<
			Ok<ContactMessageResponse>,
			ValidationProblem,
			BadRequest<ApiProblemDetails>,
			Conflict<ApiProblemDetails>,
			ProblemHttpResult>>
		SendMessageAsync(
			[FromBody] ContactMessageRequest request,
			[FromServices] ILogger<ContactEndpoints> logger,
			[FromServices] ITenantContext tenantContext,
			[FromServices] IMediator mediator,
			[FromServices] IClock clock,
			HttpContext httpContext,
			CancellationToken ct)
	{
		Dictionary<string, string[]> errors = ValidateRequest(request);

		if (errors.Count > 0)
		{
			return TypedResults.ValidationProblem(errors);
		}

		if (!tenantContext.TenantId.HasValue || tenantContext.TenantId.Value == Guid.Empty)
		{
			return TypedResults.Conflict(
				ApiProblemDetails.CreateProblem(
					StatusCodes.Status409Conflict,
					"Tenant context is missing.",
					"The message cannot be accepted because tenant context was not resolved."));
		}

		DateTimeOffset receivedAtUtc = clock.UtcNow;
		TenantId tenantId = new(tenantContext.TenantId.Value);
		string payloadJson = JsonSerializer.Serialize(
			new ContactMessageInboxPayload(
				request.Name.Trim(),
				request.EmailAddress.Trim(),
				request.Subject.Trim(),
				request.Message.Trim(),
				request.AgreeToBeContacted,
				request.FormStartedAtUtc,
				receivedAtUtc));

		logger.LogInformation(
			"Contact message received at {ReceivedAtUtc} with subject length {SubjectLength} and message length {MessageLength}.",
			receivedAtUtc,
			request.Subject.Trim().Length,
			request.Message.Trim().Length);

		CreateInboxMessageCommand command = new(
			tenantId,
			ContactMessageType,
			payloadJson,
			ContactMessageSource,
			httpContext.Request.Path,
			httpContext.Request.Method,
			$"{ContactMessageType}:{Guid.NewGuid():N}",
			receivedAtUtc,
			httpContext.Connection.RemoteIpAddress?.ToString(),
			httpContext.Request.Headers.UserAgent.ToString());

		TypedResult<CreateInboxMessageResult> result = await mediator.Send(command, ct);

		if (!result.IsSuccess || result.Value is null)
		{
			return TypedResults.Problem(
				title: "Message could not be accepted.",
				detail: result.Error?.Message,
				statusCode: MapToStatusCode(result));
		}

		logger.LogInformation(
			"Contact message accepted with reference {MessageReference}.",
			result.Value.PublicReference);

		return TypedResults.Ok(result.Value.MapToResponse());
	}

	private static Dictionary<string, string[]> ValidateRequest(ContactMessageRequest request)
	{
		ValidationContext validationContext = new(request);
		List<ValidationResult> validationResults = [];

		if (Validator.TryValidateObject(request, validationContext, validationResults, true))
		{
			return [];
		}

		return validationResults
			.SelectMany(
				static validationResult => validationResult.MemberNames.DefaultIfEmpty(string.Empty),
				static (validationResult, memberName) => new
				{
					MemberName = memberName,
					ErrorMessage = validationResult.ErrorMessage ?? "The field is invalid."
				})
			.GroupBy(static validationError => validationError.MemberName)
			.ToDictionary(
				static group => group.Key,
				static group => group.Select(static validationError => validationError.ErrorMessage).ToArray());
	}

	private static int MapToStatusCode<T>(TypedResult<T> result)
	{
		if (result.Error is null || string.IsNullOrEmpty(result.Error.Code))
		{
			return StatusCodes.Status400BadRequest;
		}

		string errorCode = result.Error.Code;

		if (errorCode.Contains("Conflict", StringComparison.OrdinalIgnoreCase))
		{
			return StatusCodes.Status409Conflict;
		}

		if (errorCode.Contains("Validation", StringComparison.OrdinalIgnoreCase))
		{
			return StatusCodes.Status400BadRequest;
		}

		return StatusCodes.Status400BadRequest;
	}

	private sealed record ContactMessageInboxPayload(
		string Name,
		string EmailAddress,
		string Subject,
		string Message,
		bool AgreeToBeContacted,
		DateTimeOffset FormStartedAtUtc,
		DateTimeOffset ReceivedAtUtc);
}