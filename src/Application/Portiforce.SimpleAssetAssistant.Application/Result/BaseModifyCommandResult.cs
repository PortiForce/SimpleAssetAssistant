namespace Portiforce.SimpleAssetAssistant.Application.Result
{
	/// <summary>
	/// Entity to capture common response information after entity has been updated
	/// </summary>
	/// <typeparam name="T">entity ID</typeparam>
	public record BaseModifyCommandResult<T> : BaseCommandResult
	{
		// here I think that we need to capture Id of the entity to be able to track history of the adjustments

		/// <summary>
		/// entity ID
		/// </summary>
		public T Id { get; init; } = default;
	}
}