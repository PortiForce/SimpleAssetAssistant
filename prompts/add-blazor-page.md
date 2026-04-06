# Add a new Blazor page in Portiforce SAA

When adding a page:

1. Find the closest existing page and mirror its structure.
2. Use:
	- loading state
	- error state
	- success/submitting state where applicable
3. Call API clients through interfaces from `Services/Interfaces`.
4. Do not embed transport logic directly in the page.
5. Use localization with `IStringLocalizer`.
6. Use shared enum/date display helpers where available.
7. Keep rendering explicit and simple.

Preferred page structure:
- `@page`
- `@using`
- `@inject`
- loading/error/content blocks
- small private methods:
	- `LoadAsync`
	- submit handlers
	- navigation helpers

For invite-related public pages:
- use `IManageInviteApiClient`
- accept/decline goes through `ManageInviteEndpoints`
- do not reuse admin-only flows or controls