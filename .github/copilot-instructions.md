# Copilot Instructions (Portiforce SAA)

> Short, always-on rules. For full rationale, priority order, and workflow see `AGENTS.md`.

## Rule priority (when rules conflict)

1. Architecture and boundary rules (layers, contracts, admin/public separation)
2. Security (OWASP), authorization, tenancy, actor-context, contract-safety
3. Nearest working example in the same feature area and layer
4. Repository configuration (`.editorconfig`, analyzers, StyleCop)

## Formatting

- Follow `.editorconfig` first.
- Use tabs in `.cs` files.
- Use spaces in `.razor`, `.json`, `.xml`, `.csproj`, `.props`, `.targets`, and `.md` files.

## Stack

- Blazor Web App (.NET 10) frontend.
- Monolith with Clean Architecture, DDD, and SOLID/OWASP principles.
- Change-first workflow: implement the change, then add or update tests in the same PR.

## Repository architecture (keep boundaries)

| Layer | Path |
|---|---|
| Domain model and invariants | `Backend/Core` |
| Use cases (commands/queries/handlers) | `Backend/App` |
| Infrastructure integrations | `Backend/Infra` |
| EF Core persistence | `Backend/Infra/Portiforce.SAA.Infrastructure.EF` |
| BFF / minimal API endpoints | `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web/Features/Endpoints` |
| Blazor UI pages and client services | `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web.Client` |
| Shared DTOs and route constants | `Shared/Portiforce.SAA.Contracts` |
| Domain tests | `Tests/Portiforce.SAA.Domain.Tests` |

## Non-negotiable rules

- Prefer additive, localized changes over broad refactors.
- Reuse existing route constants, DTOs, handlers, mappers, API clients, and result models.
- Do not move business logic into endpoints, UI/pages, mappers, or clients.
- Do not leak domain entities into contracts, endpoints, or Blazor pages.
- Keep admin invite flows and public invite flows separate.
- Do not introduce preview-only or experimental C# syntax unless the project already requires it.
- Mirror the nearest working example in the same layer unless it conflicts with architecture, security, tenancy, or contract boundaries.

## Cross-cutting (security and correctness)

- Keep authorization, tenancy, and actor-context checks explicit and early at the boundary.
- Do not log secrets, tokens, invite tokens, or sensitive personal data.
- Keep public error responses safe; do not expose internal exception details.
- Propagate `CancellationToken` through async I/O where supported.
- Do not swallow exceptions; translate them at the appropriate boundary.
- Prefer backward-compatible contract changes unless the task explicitly requires a breaking change.

## Domain (`Backend/Core`)

- Use strong IDs from `Core.Primitives.Ids`.
- Keep invariants in domain types or dedicated guard/rule classes.
- Prefer static factory methods such as `Create(...)`.
- Use private constructors only where EF Core requires them.
- No transport or infrastructure concerns in domain code.

## Application (`Backend/App`)

- Use mediator-style command/query handlers; keep commands and queries separate.
- Return projections or typed result wrappers (avoid exposing entities).
- Keep orchestration in handlers and flow services; keep mapping explicit.
- Before adding a new service, check whether a flow service, guard, or mapper already exists nearby.

## Mapping

- Use explicit static mapper classes and extension methods.
- One mapping method per response or projection shape.
- Translate enums with switch expressions.
- Map application projections to contract DTOs at the web/shared boundary.

## Endpoints (BFF)

- Endpoint classes implement `IEndpoint`; group with `MapGroup(...)`.
- Use shared route constants from contracts (no hardcoded strings).
- Return typed minimal API results.
- Validate route values and tenant/auth context early; delegate to handlers.
- Use `ApiProblemDetails` or typed problem responses for public failures.

## Blazor UI (.NET 10 Blazor Web App)

- Use API clients via interfaces from `Services/Interfaces`.
- Keep pages focused on loading, error, success/submitting, display, and navigation state.
- Do not embed transport logic directly in pages.
- Use localization with `IStringLocalizer`.
- Use shared enum/date display helpers where available.

## Invite flow rules

- Keep admin invite flows and public invite flows separate.
- Public invite UI uses `IManageInviteApiClient` and `ManageInviteEndpoints`.
- Admin invite UI uses `IAdminApiClient` and admin invite endpoints.
- Do not reuse admin-only controls or assumptions in public invite pages.

## EF Core

- Place repositories in `Backend/Infra/Portiforce.SAA.Infrastructure.EF/Repositories`.
- Add or reuse interfaces in `Backend/App/.../Interfaces/Persistence`.
- Keep repository methods persistence-focused.
- Keep tenant filtering and identity filtering explicit.
- Do not move business rules into repositories.

## Contracts (`Shared/Portiforce.SAA.Contracts`)

- Keep DTOs transport-only; no domain behavior.
- Reuse existing contract enums where possible.
- Add explicit mappers from application projections to DTOs.
- Do not expose domain entities or infrastructure types in contracts.

## Testing

- Change-first workflow: implement the change, then add or update tests in the same PR.
- Use xUnit and FluentAssertions.
- Test names: `Method_WhenCondition_ShouldExpectedResult`.
- Prefer `[Theory]` with `[InlineData]` for validation matrices.
- Assert parameter names for argument validation where relevant.
- Keep tests explicit and behavior-focused.
- Follow the nearest existing test file before introducing new helpers or patterns.
