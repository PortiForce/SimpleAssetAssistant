# Add a BFF endpoint

## Goal
Add a new minimal API endpoint in the BFF layer that delegates to a mediator handler and returns a typed result.

## Where
- Endpoint: `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web/Features/Endpoints`
- Route constant: `Shared/Portiforce.SAA.Contracts`
- Handler: `Backend/App` (should already exist or be added first)

## Steps
1. Confirm the command/query handler exists. If not, add it first (see `add-command-handler.md` or `add-query-handler.md`).
2. Add or verify the route constant in `Shared/Portiforce.SAA.Contracts`.
3. Create an endpoint class implementing `IEndpoint`.
4. Use `MapGroup(...)` and the shared route constant.
5. Validate route values and tenant/auth context early.
6. Delegate business logic to the mediator handler.
7. Map application projections to shared contract DTOs explicitly.
8. Return typed minimal API results. Use `ApiProblemDetails` or typed problem responses for failures.
9. Apply authorization policies explicitly where needed.
10. Update the API client if this endpoint serves a Blazor page (see `add-api-client.md`).

## Don'ts
- Do not place business logic in the endpoint.
- Do not leak domain entities into the response.
- Do not mix admin and public invite flows in the same endpoint group.
- Do not hardcode route strings.

## Definition of Done
- Endpoint compiles and is registered.
- Route constant is used (not a magic string).
- Authorization policy is applied where required.
- Corresponding API client method exists or is updated.
- Handler tests cover the business logic.

## Questions
- Which handler does this endpoint call?
- What authorization policy applies?
- Is this an admin or public endpoint?

Prefer existing examples: `InviteEndpoints`, `ManageInviteEndpoints`.