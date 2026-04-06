# Add a query handler in Portiforce SAA

When adding a query handler:

1. Place the query in `Backend/App/.../Actions/Queries`.
2. Place the handler in `Backend/App/.../Handlers/Queries`.
3. Return projections, not entities, unless the surrounding feature already does so.
4. Keep data access explicit through repository interfaces.
5. Use typed result wrappers if the surrounding feature uses them.
6. Keep validation, guard checks, and tenancy checks explicit.
7. Use mapper/projection helpers where already established.

Prefer nearby examples in the same feature area before adding a new pattern.