using Portiforce.SAA.Core.Extensions;
using Portiforce.SAA.Core.Interfaces;

namespace Portiforce.SAA.Core.Primitives.Ids;

public readonly record struct AccountId(Guid Value) : IGuidId<AccountId>
{
	public bool IsEmpty => this.Value == Guid.Empty;

	public static AccountId Empty => new(Guid.Empty);

	public static AccountId New() => new(GuidExtensions.New());

	public static AccountId From(Guid value) => new(value);

	public static AccountId Parse(string raw) => GuidIdParser.Parse(raw, From, nameof(AccountId));

	public static bool TryParse(string? raw, out AccountId id) => GuidIdParser.TryParse(raw, From, Empty, out id);

	public override string ToString() => GuidExtensions.ToString(this.Value);
}