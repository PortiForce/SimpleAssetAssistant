using Portiforce.SAA.Core.Primitives.Ids;
using Portiforce.SAA.Domain.Tests.Core.Primitives.Ids.Common;

namespace Portiforce.SAA.Domain.Tests.Core.Primitives.Ids;

public sealed class AssetIdTests : StronglyTypedGuidIdTests<AssetId>
{
	protected override AssetId Empty => AssetId.Empty;

	protected override AssetId New() => AssetId.New();

	protected override AssetId From(Guid value) => AssetId.From(value);

	protected override AssetId Parse(string raw) => AssetId.Parse(raw);

	protected override bool TryParse(string? raw, out AssetId id) => AssetId.TryParse(raw, out id);

	protected override Guid GetValue(AssetId id) => id.Value;

	protected override bool IsEmpty(AssetId id) => id.IsEmpty;
}
