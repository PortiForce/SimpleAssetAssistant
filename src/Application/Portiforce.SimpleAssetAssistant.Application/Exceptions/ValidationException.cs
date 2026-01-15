using System.ComponentModel.DataAnnotations;

namespace Portiforce.SimpleAssetAssistant.Application.Exceptions
{
	public class ValidationException : Exception
	{
		public List<string> Errors { get; set; } = [];

		public ValidationException(ValidationResult validationResult)
		{
			//foreach (var validationResultError in validationResult.Errors)
			//{
			//	var errorCode = validationResultError.ErrorCode;

			//	// todo alex: convert error into localized message
			//	Errors.Add(validationResultError.ErrorMessage);
			//}
		}
	}
}
