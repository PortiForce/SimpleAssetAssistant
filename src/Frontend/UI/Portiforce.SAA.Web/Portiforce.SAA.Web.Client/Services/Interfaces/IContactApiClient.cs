using Portiforce.SAA.Contracts.Models.Client.Contact;

namespace Portiforce.SAA.Web.Client.Services.Interfaces;

public interface IContactApiClient
{
	Task<ContactMessageResponse> SendMessageAsync(
		ContactMessageRequest request,
		CancellationToken ct = default);
}
