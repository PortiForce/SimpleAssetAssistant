namespace Portiforce.SimpleAssetAssistant.Application.Responses
{
	/// <summary>
	/// Entity to capture common response information after entity has been updated
	/// </summary>
	/// <typeparam name="T">entity ID</typeparam>
	public class BaseModifyCommandResponse<T> : BaseCommandResponse
	{
		// here I think that we need to capture Id of the entity to be able to track history of the adjustments

		/// <summary>
		/// entity ID
		/// </summary>
		public T Id { get; set; }
	}
}