namespace Portiforce.SimpleAssetAssistant.Core.Interfaces;

public interface IEntity<out TId>
{
	TId Id { get; }
}