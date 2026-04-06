# Add a BFF endpoint in Portiforce SAA

When adding a BFF endpoint:

1. Place it in `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web/Features/Endpoints`.
2. Follow the existing `IEndpoint` pattern.
3. Use `MapGroup(...)` and shared route constants.
4. Return typed minimal API results.
5. Validate route values and tenant/auth context early.
6. Delegate business logic to mediator handlers.
7. Map application projections to shared contract DTOs explicitly.

Prefer existing examples:
- `InviteEndpoints`
- `ManageInviteEndpoints`

Do not:
- place business logic in the endpoint
- leak domain entities to the response
- mix admin and public invite flows