# Add or update an API client method

## Goal
Add a new method (or update an existing one) in a Blazor client API service so a page can call a BFF endpoint.

## Where
- Client service: `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web.Client/Services`
- Interface: `Services/Interfaces`
- Route constants: `Shared/Portiforce.SAA.Contracts`

## Steps
1. Confirm the BFF endpoint and its route constant already exist. If not, add the endpoint first (see `add-bff-endpoint.md`).
2. Add or locate the client interface in `Services/Interfaces` (e.g., `IAdminApiClient`, `IManageInviteApiClient`).
3. Add the method signature to the interface. Accept a `CancellationToken` parameter.
4. Implement the method in the concrete client class that inherits from `ApiClientBase`.
5. Use the shared route constant — do not hardcode the URL.
6. Return a typed result consistent with nearby methods (e.g., the same `ApiResult<T>` or response DTO).
7. If this is a new client class, register it in DI.
8. Update or add a page that calls the new method, if the task requires it.

## Don'ts
- Do not put HTTP/transport logic directly in Blazor pages.
- Do not create a new client class when an existing one covers the same feature area.
- Do not hardcode route strings — use constants from `Shared/Portiforce.SAA.Contracts`.
- Do not mix admin and public invite clients (`IAdminApiClient` vs `IManageInviteApiClient`).

## Definition of Done
- Interface and implementation compile.
- Route constant is referenced (not a magic string).
- `CancellationToken` is propagated.
- At least one page or test exercises the new method.

## Questions
- Which BFF endpoint does this client method call?
- Is this an admin or public flow?
- What response type should the method return?