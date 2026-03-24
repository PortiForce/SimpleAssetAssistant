namespace Portiforce.SAA.Core.Interfaces;

public interface IGuidId<TSelf>
	where TSelf : struct, IGuidId<TSelf>
{
	Guid Value { get; }

	bool IsEmpty { get; }

	static abstract TSelf Empty { get; }

	static abstract TSelf New();

	static abstract TSelf From(Guid value);

	static abstract TSelf Parse(string raw);

	static abstract bool TryParse(string? raw, out TSelf id);
}