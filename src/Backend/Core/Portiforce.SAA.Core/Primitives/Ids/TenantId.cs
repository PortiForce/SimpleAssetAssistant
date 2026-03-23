using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct TenantId(Guid Value) : IGuidId<TenantId>
{
	public static TenantId New() => new(GuidExtensions.New());
	public static TenantId Empty => new(Guid.Empty);
	public static TenantId From(Guid value) => new(value);
	public bool IsEmpty => Value == Guid.Empty;
	public override string ToString() => GuidExtensions.ToString(Value);

	public static TenantId Parse(string raw)
		=> GuidIdParser.Parse(raw, From, nameof(TenantId));

	public static bool TryParse(string? raw, out TenantId id)
		=> GuidIdParser.TryParse(raw, From, Empty, out id);
}
