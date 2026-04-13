# Add a Blazor page

## Goal
Add a new Blazor page that calls an API client and renders state (loading, error, success) cleanly.

## Where
- Page: `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web.Client` (appropriate feature folder)
- Client interface: `Services/Interfaces`

## Steps
1. Find the closest existing page in the same feature area and mirror its structure.
2. Create the `.razor` file with the standard block order:
   - `@page`
   - `@using`
   - `@inject`
   - Loading / error / content blocks
3. Inject the API client through its interface (e.g., `IAdminApiClient`, `IManageInviteApiClient`).
4. Handle explicit states: loading, error, success/submitting, display, navigation.
5. Use localization with `IStringLocalizer`.
6. Use shared enum/date display helpers where available.
7. Keep rendering explicit and simple — small private methods for `LoadAsync`, submit handlers, navigation helpers.
8. Use `InteractiveWebAssemblyRenderMode(prerender: false)` where existing pages do.

## Don'ts
- Do not embed HTTP/transport logic directly in the page.
- Do not call admin API clients from public invite pages (or vice versa).
- Do not add business logic to the page — delegate to the API client and backend handlers.
- Do not skip error or loading states.

## Definition of Done
- Page renders correctly in all states (loading, error, success).
- API client is called through its interface.
- Localized strings are used for user-facing text.
- No transport logic in the page.

## Questions
- Which API client and endpoint does this page use?
- Is this an admin or public page?
- Are there existing pages in the same feature area to mirror?