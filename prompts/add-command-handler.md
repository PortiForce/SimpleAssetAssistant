# Add a command handler in Portiforce SAA

When adding a command handler:

1. Place the command in `Backend/App/.../Actions/Commands`.
2. Place the handler in `Backend/App/.../Handlers/Commands`.
3. Use mediator handler conventions already present in the solution.
4. Keep orchestration in the handler, not in endpoints or UI.
5. Use repositories, flow services, and guards explicitly.
6. Return typed result or result model consistent with nearby handlers.
7. Keep mapping and transport concerns out of the handler.

Before adding a new service:
- check if a flow service, guard, or mapper already exists nearby