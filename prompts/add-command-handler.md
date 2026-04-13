# Add a command handler

## Goal
Add a new command and its handler to execute a write operation in the application layer.

## Where
- Command: `Backend/App/.../Actions/Commands`
- Handler: `Backend/App/.../Handlers/Commands`

## Steps
1. Place the command record in `Actions/Commands` within the appropriate feature folder.
2. Place the handler in `Handlers/Commands` in the same feature folder.
3. Use mediator handler conventions already present in the solution.
4. Accept a `CancellationToken` parameter and propagate it through async calls.
5. Use repositories, flow services, and guards explicitly.
6. Keep orchestration in the handler — not in endpoints or UI.
7. Return a typed result or result model consistent with nearby handlers.
8. Keep mapping and transport concerns out of the handler.
9. Add or update tests covering valid and invalid paths (see `add-tests.md`).

## Don'ts
- Do not place business logic in endpoints, mappers, or pages.
- Do not expose domain entities in the return type.
- Do not swallow exceptions — translate at the boundary.
- Do not duplicate existing flow services, guards, or mappers without checking first.

## Definition of Done
- Command and handler compile and are registered.
- `CancellationToken` is propagated.
- Return type is consistent with nearby handlers.
- Tests cover both success and failure paths.

## Questions
- Which feature area does this command belong to?
- What repositories or flow services does it need?
- What result type should the handler return?