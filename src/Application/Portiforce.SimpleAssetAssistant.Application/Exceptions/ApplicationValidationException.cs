using System.ComponentModel.DataAnnotations;

namespace Portiforce.SimpleAssetAssistant.Application.Exceptions;

public sealed class ApplicationValidationException : Exception
{
	private readonly List<string> _errors = new();

	public IReadOnlyList<string> Errors => _errors.AsReadOnly();

	public ApplicationValidationException(ValidationResult validationResult)
	{
		if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
		{
			_errors.Add(validationResult.ErrorMessage);
		}
	}

	public ApplicationValidationException(IEnumerable<ValidationResult> validationResults)
	{
		foreach (var result in validationResults)
		{
			if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
			{
				_errors.Add(result.ErrorMessage);
			}
		}
	}
}