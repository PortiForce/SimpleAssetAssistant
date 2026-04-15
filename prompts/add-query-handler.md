# Add a query handler

## Goal
Add a new query and its handler to retrieve data through the application layer.

## Where
- Query: `Backend/App/.../Actions/Queries`
- Handler: `Backend/App/.../Handlers/Queries`

## Steps
1. Place the query record in `Actions/Queries` within the appropriate feature folder.
2. Place the handler in `Handlers/Queries` in the same feature folder.
3. Return projections — not entities — unless the surrounding feature already does so.
4. Keep data access explicit through repository interfaces.
5. Accept a `CancellationToken` parameter and propagate it through async calls.
6. Use typed result wrappers if the surrounding feature uses them.
7. Keep validation, guard checks, and tenancy checks explicit.
8. Use mapper/projection helpers where already established.
9. Add or update tests covering valid and invalid paths (see `add-tests.md`).

## Don'ts
- Do not return raw domain entities from the handler.
- Do not bypass tenant or identity filtering.
- Do not place query logic in endpoints or pages.
- Do not duplicate existing projections or mappers without checking first.

## Definition of Done
- Query and handler compile and are registered.
- `CancellationToken` is propagated.
- Return type is a projection or typed result (not an entity).
- Tenancy/identity filtering is explicit.
- Tests cover both success and edge-case paths.

## Questions
- Which feature area does this query belong to?
- What projection shape should the handler return?
- Are there existing queries in the same area to mirror?

Prefer nearby examples in the same feature area before adding a new pattern.