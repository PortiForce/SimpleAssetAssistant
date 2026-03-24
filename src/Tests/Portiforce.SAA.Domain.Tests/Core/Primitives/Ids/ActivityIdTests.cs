using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class ActivityIdTests : StronglyTypedGuidIdTests<ActivityId>
{
	protected override ActivityId Empty => ActivityId.Empty;

	protected override ActivityId New() => ActivityId.New();

	protected override ActivityId From(Guid value) => ActivityId.From(value);

	protected override ActivityId Parse(string raw) => ActivityId.Parse(raw);

	protected override bool TryParse(string? raw, out ActivityId id) => ActivityId.TryParse(raw, out id);

	protected override Guid GetValue(ActivityId id) => id.Value;

	protected override bool IsEmpty(ActivityId id) => id.IsEmpty;
}
