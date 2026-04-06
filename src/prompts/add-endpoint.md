# Add a new endpoint in Portiforce SAA

When adding a new endpoint:

1. Determine the correct layer:
	- public invite/BFF endpoint -> `Frontend/UI/Portiforce.SAA.Web/Features/Endpoints`
	- API controller endpoint -> `Backend/Api`
2. Reuse route constants from `Shared/Portiforce.SAA.Contracts.Configuration`.
3. Implement endpoint class style consistent with existing `IEndpoint` classes:
	- `MapGroup(...)`
	- typed `Results<...>`
	- early validation
	- mediator call
	- explicit mapping to contract response
4. Do not place business logic in the endpoint.
5. If needed, add:
	- request/command/query
	- handler
	- projection/result type
	- mapper
	- API client method
	- Blazor page integration

Formatting rules:
- tabs in `.cs`
- file-scoped namespaces
- explicit types preferred
- braces required

Prefer existing examples:
- `InviteEndpoints`
- `ManageInviteEndpoints`