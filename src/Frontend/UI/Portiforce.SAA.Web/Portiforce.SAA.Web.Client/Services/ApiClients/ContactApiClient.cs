using Portiforce.SAA.Contracts.Configuration;
using Portiforce.SAA.Contracts.Models.Client.Contact;
using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services.ApiClients;

public sealed class ContactApiClient(HttpClient httpClient) : ApiClientBase(httpClient), IContactApiClient
{
	public async Task<ContactMessageResponse> SendMessageAsync(
		ContactMessageRequest request,
		CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		return await this.PostJsonAsync<ContactMessageRequest, ContactMessageResponse>(
			ApiRoutes.Contact,
			request,
			ct);
	}
}
