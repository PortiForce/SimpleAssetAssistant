using System.ComponentModel.DataAnnotations;

namespace Portiforce.SimpleAssetAssistant.Application.Exceptions;

public class ApplicationValidationException : Exception
{
	public List<string> Errors { get; set; } = [];

	public ApplicationValidationException(ValidationResult validationResult)
	{
		if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
		{
			Errors.Add(validationResult.ErrorMessage);
		}
	}

	public ApplicationValidationException(IEnumerable<ValidationResult> validationResults)
	{
		foreach (var result in validationResults)
		{
			if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
			{
				Errors.Add(result.ErrorMessage);
			}
		}
	}
}