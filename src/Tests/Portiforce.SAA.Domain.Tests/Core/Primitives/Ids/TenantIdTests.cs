using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class TenantIdTests : StronglyTypedGuidIdTests<TenantId>
{
	protected override TenantId Empty => TenantId.Empty;

	protected override TenantId New() => TenantId.New();

	protected override TenantId From(Guid value) => TenantId.From(value);

	protected override TenantId Parse(string raw) => TenantId.Parse(raw);

	protected override bool TryParse(string? raw, out TenantId id) => TenantId.TryParse(raw, out id);

	protected override Guid GetValue(TenantId id) => id.Value;

	protected override bool IsEmpty(TenantId id) => id.IsEmpty;
}
