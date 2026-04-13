# Add an invite flow

## Goal
Add or extend an invite-related feature, keeping admin and public flows strictly separate.

## Where

### Admin invite flow
- Endpoints: `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web/Features/Endpoints` (admin invite endpoints)
- Client: `IAdminApiClient` / `AdminApiClient`
- Pages: admin area in Blazor client

### Public invite flow
- Endpoints: `ManageInviteEndpoints`
- Client: `IManageInviteApiClient` / `ManageInviteApiClient`
- Pages: public invite area in Blazor client

### Shared
- DTOs and route constants: `Shared/Portiforce.SAA.Contracts`
- Commands/queries: `Backend/App`
- Domain: `Backend/Core`

## Steps
1. Determine whether this is an **admin** flow or a **public** flow. Do not mix them.
2. Add or update the route constant in `Shared/Portiforce.SAA.Contracts`.
3. Add or update the command/query and handler in `Backend/App`.
4. Add or update the BFF endpoint in the appropriate endpoint group.
5. Add or update the API client method in the correct client (`AdminApiClient` or `ManageInviteApiClient`).
6. Add or update the Blazor page, using the correct client interface.
7. Add or update tests for the handler and domain logic.

## Don'ts
- Do not reuse admin-only controls, endpoints, or assumptions in public invite pages.
- Do not reuse public invite clients or endpoints for admin use cases.
- Do not expose invite tokens in logs or public error responses.
- Do not place invite business logic in endpoints or pages.

## Definition of Done
- The flow works end-to-end: endpoint → client → page.
- Admin and public flows remain completely separate.
- Route constants are used (no hardcoded strings).
- Invite tokens are never logged.
- Tests cover the handler logic.

## Questions
- Is this an admin flow or a public flow?
- Which invite operations are involved (list, details, summary, resend, revoke, overview, accept, decline)?
- Are there existing invite endpoints or clients to extend?
