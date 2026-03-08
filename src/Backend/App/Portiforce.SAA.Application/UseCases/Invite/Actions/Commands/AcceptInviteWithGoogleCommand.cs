using Portiforce.SAA.Application.FlowResult;
using Portiforce.SAA.Application.Tech.Abstractions.Messaging;
using Portiforce.SAA.Application.UseCases.Invite.Result;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Application.UseCases.Invite.Actions.Commands;

/// <summary>
/// Represents a command to accept an invitation using Google authentication for a specific tenant.
/// </summary>
/// <remarks>Use this command when a user is invited to join a tenant via Google authentication. All parameters
/// are required to process the acceptance and establish the user's identity within the system.</remarks>
/// <param name="TenantId">The unique identifier of the tenant associated with the invitation.</param>
/// <param name="RawToken">The raw authentication token received from Google, used to validate the invitation.</param>
/// <param name="Email">The email address of the user accepting the invitation.</param>
/// <param name="GoogleSubjectId">The unique subject identifier for the user in Google, used for authentication purposes.</param>
/// <param name="FirstName">The first name of the user accepting the invitation.</param>
/// <param name="LastName">The last name of the user accepting the invitation.</param>
public sealed record AcceptInviteWithGoogleCommand(
	TenantId TenantId,
	string RawToken,
	string Email,
	string GoogleSubjectId,
	string FirstName,
	string LastName) : ICommand<TypedResult<AcceptInviteResult>>;
