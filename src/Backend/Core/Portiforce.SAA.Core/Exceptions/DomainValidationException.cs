namespace Portiforce.SAA.Core.Exceptions;

public sealed class DomainValidationException : Exception
{
	public DomainValidationException(string message) : base(message) { }
}
