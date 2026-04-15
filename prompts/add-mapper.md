# Add a mapper

## Goal
Add or extend explicit mapping logic between layers (application projections ↔ contract DTOs, domain ↔ projections).

## Where
- Mapper class: same layer as the consumer (typically `Backend/App` or BFF endpoint layer)
- Keep boundary mapping (projection → DTO) at the web/shared boundary

## Steps
1. Check if an existing mapper class in the feature area already covers this shape. If so, extend it.
2. Otherwise, add a new static mapper class with extension methods.
3. Keep mapping explicit and one-way per use case — one method per projection or response shape.
4. Translate enums with switch expressions.
5. Map application projections to contract DTOs at the web/shared boundary.
6. Keep mappings simple — no hidden business logic.

## Don'ts
- Do not hide complex business logic inside mapping methods.
- Do not use reflection-based or convention-based mapping libraries unless the project already does.
- Do not map domain entities directly to contract DTOs (go through a projection).
- Do not duplicate mapping logic that already exists nearby.

## Definition of Done
- Mapper compiles and is used by the endpoint or handler that needs it.
- Each mapping method handles one shape explicitly.
- Enum translations are exhaustive switch expressions.
- No business logic is hidden in the mapper.

## Questions
- What source and target types are being mapped?
- Does an existing mapper in this feature area cover a similar shape?
- Are there enums that need translation?

Prefer existing examples: `InviteMapper`, `AccountMapper`.