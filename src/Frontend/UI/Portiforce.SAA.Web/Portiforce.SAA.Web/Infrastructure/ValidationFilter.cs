using System.ComponentModel.DataAnnotations;

namespace Portiforce.SAA.Web.Infrastructure;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var argument = context.Arguments.OfType<T>().FirstOrDefault();

		if (argument is not null)
		{
			var validationContext = new ValidationContext(argument);
			var validationResults = new List<ValidationResult>();

			bool isValid = Validator.TryValidateObject(
				argument,
				validationContext,
				validationResults,
				validateAllProperties: true);

			if (!isValid)
			{
				var errors = validationResults
					.GroupBy(e => e.MemberNames.FirstOrDefault() ?? string.Empty)
					.ToDictionary(
						g => g.Key,
						g => g.Select(e => e.ErrorMessage ?? "Invalid value.").ToArray()
					);

				return TypedResults.ValidationProblem(errors);
			}
		}
		
		return await next(context);
	}
}