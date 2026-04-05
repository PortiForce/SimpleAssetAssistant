using Portiforce.SAA.Web.Client.Services.Interfaces;

namespace Portiforce.SAA.Web.Client.Services.ApiClients;

public sealed class InviteSummaryApiClient(HttpClient httpClient) : ApiClientBase(httpClient), IInviteSummaryApiClient
{
}