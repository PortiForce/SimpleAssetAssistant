using Portiforce.SAA.Contracts.Enums;

namespace Portiforce.SAA.Contracts.Models.Client.Invite.Summary;

public sealed record InviteTrendResponse(
	InviteTrendBucket Bucket,
	IReadOnlyList<InviteTrendPointResponse> Points);