using Portiforce.SAA.Application.Interfaces.Persistence.Invite;
using Portiforce.SAA.Application.UseCases.Invite.Projections.Summary;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Infrastructure.EF.DbContexts;

namespace Portiforce.SAA.Infrastructure.EF.Repositories.Invite;

internal sealed class InviteSummaryRepository(AssetAssistantDbContext db) : IInviteSummaryRepository
{
	public Task<InviteSummary> GetSummaryAsync(
		TenantId tenantId,
		HashSet<InviteChannel>? channels,
		HashSet<InviteState>? states,
		DateTime? fromDate,
		DateTime? toDate,
		bool? hasAccount,
		bool? includeRevoked,
		InviteSummaryRange range,
		InviteTrendBucket trendBucket,
		CancellationToken ct)
	{
		DateTimeOffset now = DateTimeOffset.UtcNow;

		(DateTimeOffset fromUtc, InviteTrendBucket bucket, IReadOnlyList<InviteTrendPoint> points) =
			range switch
			{
				InviteSummaryRange.Today => (
					now.Date,
					InviteTrendBucket.Hour,
					BuildHourlyPoints(now.Date, 24)),

				InviteSummaryRange.LastWeek => (
					now.Date.AddDays(-6),
					InviteTrendBucket.Day,
					BuildDailyPoints(now.Date.AddDays(-6), 7)),

				InviteSummaryRange.LastMonth => (
					now.Date.AddDays(-28),
					InviteTrendBucket.Week,
					BuildWeeklyPoints(now.Date.AddDays(-28), 5)),

				InviteSummaryRange.AllTime => (
					now.Date.AddMonths(-11),
					InviteTrendBucket.Month,
					BuildMonthlyPoints(
						new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(-11),
						12)),

				_ => (
					now.Date,
					InviteTrendBucket.Hour,
					BuildHourlyPoints(now.Date, 24))
			};

		InviteSummaryCards cards = new(
			180,
			96,
			18,
			12,
			34,
			14,
			6,
			53.33m);

		InviteOutcomeBreakdown breakdown = new(
			96,
			18,
			12,
			34,
			14,
			6);

		IReadOnlyList<InviteTierUsage> tierUsage =
		[
			new("Free", 18, 82, 100, 18m, 82m),
			new("Standard", 67, 133, 200, 33.5m, 66.5m),
			new("Premium", 95, 405, 500, 19m, 81m)
		];

		InviteSummary response = new(
			range,
			trendBucket,
			fromUtc,
			now,
			cards,
			breakdown,
			tierUsage,
			new InviteTrend(bucket, points));

		return Task.FromResult(response);
	}

	private static IReadOnlyList<InviteTrendPoint> BuildHourlyPoints(DateTimeOffset start, int count)
	{
		List<InviteTrendPoint> points = [];

		for (int i = 0; i < count; i++)
		{
			points.Add(
				new InviteTrendPoint(
					start.AddHours(i),
					2 + (i % 4),
					i % 3,
					i % 2 == 0 ? 1 : 0,
					i % 7 == 0 ? 1 : 0,
					3 + (i % 2),
					i % 11 == 0 ? 1 : 0,
					i % 13 == 0 ? 1 : 0));
		}

		return points;
	}

	private static IReadOnlyList<InviteTrendPoint> BuildDailyPoints(DateTimeOffset start, int count)
	{
		List<InviteTrendPoint> points = [];

		for (int i = 0; i < count; i++)
		{
			points.Add(
				new InviteTrendPoint(
					start.AddDays(i),
					10 + (i * 2),
					4 + i,
					1 + (i % 2),
					i % 4 == 0 ? 1 : 0,
					3 + (i % 3),
					i % 5 == 0 ? 1 : 0,
					i % 6 == 0 ? 1 : 0));
		}

		return points;
	}

	private static IReadOnlyList<InviteTrendPoint> BuildWeeklyPoints(DateTimeOffset start, int count)
	{
		List<InviteTrendPoint> points = [];

		for (int i = 0; i < count; i++)
		{
			points.Add(
				new InviteTrendPoint(
					start.AddDays(i * 7),
					20 + (i * 8),
					12 + (i * 4),
					3 + i,
					1 + (i % 2),
					5 + (i % 3),
					1,
					i % 3 == 0 ? 1 : 0));
		}

		return points;
	}

	private static IReadOnlyList<InviteTrendPoint> BuildMonthlyPoints(DateTimeOffset start, int count)
	{
		List<InviteTrendPoint> points = [];

		for (int i = 0; i < count; i++)
		{
			points.Add(
				new InviteTrendPoint(
					start.AddMonths(i),
					40 + (i * 10),
					24 + (i * 5),
					4 + (i % 4),
					2 + (i % 2),
					6 + (i % 3),
					2,
					1));
		}

		return points;
	}
}