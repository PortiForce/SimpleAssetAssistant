# Add a shared contract DTO

## Goal
Add a new request, response, or route-constant DTO to the shared contracts project.

## Where
- DTOs: `Shared/Portiforce.SAA.Contracts` (appropriate feature folder)
- Route constants: `Shared/Portiforce.SAA.Contracts/Configuration`

## Steps
1. Place the DTO under `Shared/Portiforce.SAA.Contracts` in the matching feature folder.
2. Keep it transport-only — a small explicit `record` for request/response models.
3. Reuse existing contract enums where possible.
4. Add route constants to the `Configuration` area if the feature needs new routes.
5. Add an explicit mapper from the application projection to this DTO (see `add-mapper.md`).
6. Prefer backward-compatible changes unless the task explicitly requires a breaking change.

## Don'ts
- Do not add domain behavior or validation logic to contract types.
- Do not expose domain entities or infrastructure types.
- Do not place UI-specific behavior inside contracts.
- Do not duplicate existing DTOs — check first.

## Definition of Done
- DTO compiles and lives in the correct contracts folder.
- Route constant exists if needed.
- An explicit mapper exists from the application projection to this DTO.
- No domain entities or infrastructure types are exposed.

## Questions
- Which feature area does this DTO belong to?
- Is this a request DTO, response DTO, or both?
- Does a similar DTO already exist that can be extended?