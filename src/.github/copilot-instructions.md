# Copilot Instructions

## Project Guidelines
- User prefers tabs instead of spaces in this codebase/file.

## Repository Architecture
- Follow existing layer boundaries before adding new code.
- Keep domain logic in `Backend/Core`.
- Keep use-case orchestration in `Backend/App`.
- Keep infrastructure integrations in `Backend/Infra`.
- Keep EF Core persistence in `Backend/Infra/Portiforce.SAA.Infrastructure.EF`.
- Keep BFF/minimal API endpoints in `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web/Features/Endpoints`.
- Keep Blazor UI pages and client services in `Frontend/UI/Portiforce.SAA.Web/Portiforce.SAA.Web.Client`.
- Keep shared request/response DTOs and route constants in `Shared/Portiforce.SAA.Contracts`.

## General Coding Rules
- Prefer additive, localized changes over broad refactors.
- Reuse existing route constants, DTOs, handlers, mappers, API clients, and result models whenever possible.
- Mirror the nearest working example in the same layer unless it conflicts with architecture, security, tenancy, or contract boundaries.
- Do not introduce preview-only or experimental C# syntax unless the project already requires it.
- Keep changes explicit and reviewable.

## Domain Rules
- Use explicit domain types and strong IDs from `Core.Primitives.Ids`.
- Keep invariants in domain types or dedicated rule/guard classes.
- Prefer static factory methods such as `Create(...)`.
- Use private constructors only where EF Core requires them.
- Do not introduce transport or infrastructure concerns into domain code.

## Application Layer Rules
- Use mediator-style command/query handlers.
- Keep commands and queries separated.
- Return projections or typed result wrappers instead of exposing entities directly.
- Keep orchestration in handlers and flow services.
- Before adding a new service, check whether a flow service, guard, or mapper already exists nearby.

## Mapping Rules
- Use explicit static mapper classes and extension methods.
- Prefer one mapping method per response or projection shape.
- Keep mappings simple and explicit.
- Translate enums with switch expressions.
- Map application projections to contract DTOs in the web/shared boundary.

## Endpoint Rules
- Use endpoint classes implementing `IEndpoint`.
- Group routes with `MapGroup(...)`.
- Return typed minimal API results.
- Validate route values and tenant/auth context early.
- Keep endpoint code thin and delegate business logic to mediator handlers.
- Do not place business logic in endpoints.
- Do not leak domain entities into responses.
- Use shared route constants from contracts instead of hardcoded strings.

## Blazor UI Rules
- Use API clients through interfaces from `Services/Interfaces`.
- Keep pages focused on loading, error, success/submitting, display, and navigation state.
- Do not embed transport logic directly in pages.
- Use localization with `IStringLocalizer`.
- Use shared enum/date display helpers where available.
- Keep rendering explicit and simple.

## Invite Flow Rules
- Keep admin invite flows and public invite flows separate.
- Public invite UI should use `IManageInviteApiClient` and `ManageInviteEndpoints`.
- Admin invite UI should use `IAdminApiClient` and admin invite endpoints.
- Do not reuse admin-only controls or assumptions in public invite pages.

## EF Core Rules
- Place repositories in `Backend/Infra/Portiforce.SAA.Infrastructure.EF/Repositories`.
- Add or reuse corresponding interfaces in `Backend/App/.../Interfaces/Persistence`.
- Keep repository methods persistence-focused.
- Keep tenant filtering and identity filtering explicit.
- Do not move business rules into repositories.

## Contract Rules
- Keep DTOs transport-only.
- Do not add domain behavior to shared contracts.
- Reuse existing contract enums where possible.
- Add explicit mappers from application projections to DTOs.
- Do not expose domain entities or infrastructure types in contracts.

## Testing Rules
- Use xUnit and FluentAssertions.
- Test names should follow `Method_WhenCondition_ShouldExpectedResult`.
- Prefer `[Theory]` with `[InlineData]` for validation matrices.
- Assert parameter names for argument validation where relevant.
- Keep tests explicit and behavior-focused.
- Follow the nearest existing test file before introducing new helpers or patterns.
