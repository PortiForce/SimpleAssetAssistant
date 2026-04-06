# Portiforce SAA Agent Guide

## Non-negotiable rules

- Follow existing layer boundaries before adding new code.
- Prefer additive, localized changes over broad refactors.
- Do not move business logic into UI, endpoints, mappers, or client layers.
- Do not leak domain entities into contracts, BFF endpoints, or Blazor pages.
- Reuse existing route constants, DTOs, handlers, mappers, API clients, and result models whenever possible.
- Keep admin invite flows and public invite flows separate.
- Keep changes small, explicit, and reviewable.
- Avoid preview-only or experimental C# syntax unless the project already requires it.
- Mirror the nearest working example in the same layer unless it conflicts with architecture boundaries, contracts, authorization, tenancy, or security expectations.

## Rule priority

When rules conflict, follow this order:

1. Architecture and boundary rules in this file
2. Security, authorization, tenancy, and contract-safety rules
3. Existing feature-local patterns in the same layer
4. Repository settings such as `.editorconfig` and analyzers
5. General style preferences in this guide

## Solution overview

- `Backend/Core`
  - domain model, value objects, entities, rules, enums
- `Backend/App`
  - commands, queries, handlers, projections, flow services, result models
- `Backend/Infra`
  - external integrations and infrastructure services
- `Backend/Infra/Portiforce.SAA.Infrastructure.EF`
  - EF Core persistence, repositories, configurations, seeders
- `Backend/Api`
  - backend API controllers and contracts
- `Frontend/UI/Portiforce.SAA.Web`
  - BFF/minimal API endpoints and server-side composition
- `Frontend/UI/Portiforce.SAA.Web.Client`
  - Blazor WebAssembly UI, pages, API clients, UI helpers
- `Shared/Portiforce.SAA.Contracts`
  - shared DTOs, routes, UI enums
- `Tests/Portiforce.SAA.Domain.Tests`
  - xUnit + FluentAssertions domain tests

## Formatting and code style

- Follow `.editorconfig` and existing repository formatting first.
- Use tabs in `.cs` files.
- Use spaces in `.razor`, `.json`, `.xml`, `.csproj`, `.props`, `.targets`, and `.md`.
- Keep file-scoped namespaces where already used.
- Keep braces on control blocks.
- Prefer explicit types over `var` where it improves clarity and matches nearby code.
- Preserve meaningful existing comments unless they become inaccurate; update or remove stale comments instead of keeping them.

## Cross-cutting expectations

- Keep authorization, tenancy, and actor-context checks explicit.
- Keep public and admin flows separate where the repository already models them separately.
- Propagate `CancellationToken` through async I/O flows where supported.
- Do not swallow exceptions; translate them at the appropriate boundary.
- Do not log secrets, tokens, invite tokens, or sensitive personal data.
- Keep public error responses safe; do not expose internal exception details.
- Prefer backward-compatible contract changes unless the task explicitly requires a breaking change.
- Do not edit generated files, migrations, props/targets, or solution-wide config unless the task requires it.

## Domain model style

- Keep invariants in domain types or dedicated rule/guard classes.
- Use explicit, strongly typed domain concepts and IDs.
- Prefer static factory methods such as `Create(...)`.
- Use private constructors only where EF Core requires them.
- Validate early with guard clauses.
- Throw domain-specific exceptions such as `DomainValidationException`.
- Do not introduce infrastructure or transport concerns into domain code.

## Application layer style

- Use mediator-style command/query handlers.
- Keep commands and queries separated.
- Return projections or typed result wrappers instead of exposing entities.
- Keep orchestration in handlers and flow services.
- Keep mapping explicit.

## Mapping style

- Use explicit static mapper classes and extension methods.
- Prefer one mapping method per response or projection shape.
- Keep mappings simple and non-magical.
- Use explicit enum translation, typically via switch expressions.

## Web/BFF endpoint style

- Use endpoint classes implementing `IEndpoint`.
- Group routes with `MapGroup(...)`.
- Use shared route constants from contracts instead of hardcoded strings.
- Keep endpoint code thin and delegate business logic to handlers.
- Apply authorization policies explicitly where needed.
- Validate route and service preconditions early.
- Return typed minimal API results.
- Use `ApiProblemDetails` or typed problem responses for public failures.

## Blazor client style

- Use API client services inheriting from `ApiClientBase`.
- Keep pages focused on loading, submission, presentation, and navigation state.
- Handle loading, error, and success states explicitly.
- Use localization via `IStringLocalizer`.
- Use enum display extensions for UI labels where already established.
- Use `InteractiveWebAssemblyRenderMode(prerender: false)` where existing pages do.
- Public invite UI should call `ManageInviteApiClient`.
- Admin UI should call `AdminApiClient`.

## EF Core style

- Keep repositories in `Infrastructure.EF`.
- Keep configurations split by bounded area.
- Keep EF-only constructors private and minimal.
- Keep persistence concerns out of domain behavior.

## Testing style

- Use xUnit and FluentAssertions.
- Test names follow `Method_WhenCondition_ShouldExpectedResult`.
- Prefer `[Theory]` with `[InlineData]` for validation matrices.
- Assert parameter names for argument validation where relevant.
- Keep tests explicit and behavior-focused.
- Follow the nearest existing test file before introducing new helpers or patterns.

## Invite flow rules

- Admin flows and public invite flows are separate.
- Admin endpoints support tenant-admin use cases such as list, details, summary, resend, and revoke.
- Public invite endpoints resolve by invite token and support overview, accept, and decline.
- Public pages must not reuse admin-only controls or assumptions.

## Agent workflow

### Before changing code

- Inspect the nearest working implementation in the same feature area and layer.
- Check whether a shared route, DTO, mapper, projection, handler, result model, or client already exists.
- Prefer extending an existing flow over creating a parallel one.

### When adding a backend feature

Add only the missing pieces in the appropriate layer:

- route constant if needed
- request/command/query
- handler
- projection or result model
- mapper
- endpoint
- client API method
- tests where appropriate

### When adding a Blazor page

- Mirror the nearest existing page structure.
- Use an injected client interface from `Services/Interfaces`.
- Keep page logic thin.
- Handle loading, error, and submit state explicitly.
- Use localized strings and existing UI helpers.

### When adding tests

- Follow the nearest relevant existing test file.
- Cover valid and invalid paths.
- Prefer explicit assertions over helper-heavy indirection.

## Prompt folder guidance

Prompt files in `prompts/` should be task-specific, short, practical, and aligned with the actual repository structure and conventions.

Each prompt should:

- identify the target layer
- point to the nearest existing examples
- reinforce route, mapper, endpoint, and testing conventions
- avoid encouraging architectural rewrites

## Known repo realities

- Analyzer configuration exists, but some StyleCop setup is still noisy or incomplete.
- Follow practical conventions from code and `.editorconfig` rather than triggering style-only rewrites.
- Some areas are still being aligned; prefer consistency with nearby code unless it violates the rules above.