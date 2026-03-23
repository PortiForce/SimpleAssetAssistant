namespace Portiforce.SAA.Core.Interfaces;

public interface IGuidId<TSelf>
	where TSelf : struct, IGuidId<TSelf>
{
	static abstract TSelf Empty { get; }
	static abstract TSelf New();
	static abstract TSelf From(Guid value);
	static abstract TSelf Parse(string raw);
	static abstract bool TryParse(string? raw, out TSelf id);

	Guid Value { get; }
	bool IsEmpty { get; }
}
