namespace Portiforce.SAA.Contracts.Models.Client.Contact;

public sealed record ContactMessageResponse(
	DateTimeOffset ReceivedAtUtc,
	string MessageReference);
