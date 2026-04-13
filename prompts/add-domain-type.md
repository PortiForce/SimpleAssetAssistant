# Add a domain type

## Goal
Add a new entity, value object, or aggregate to the domain model in `Backend/Core`.

## Where
- Domain types: `Backend/Core` (in the appropriate bounded-context folder)
- Strong IDs: `Core.Primitives.Ids`

## Steps
1. Identify the bounded context and folder where this type belongs.
2. Create the type with a strong ID from `Core.Primitives.Ids` if it is an entity or aggregate root.
3. Add a static factory method such as `Create(...)` that enforces all required invariants.
4. Add a private parameterless constructor only if EF Core requires it for materialization.
5. Keep invariants inside the type itself or in a dedicated guard/rule class nearby.
6. Validate early with guard clauses; throw domain-specific exceptions (e.g., `DomainValidationException`).
7. Add or update tests covering valid creation and invalid/guarded paths (see `add-tests.md`).

## Don'ts
- Do not introduce transport or infrastructure concerns (HTTP, EF, serialization attributes) into domain code.
- Do not expose public setters that bypass invariants.
- Do not add behavior that belongs in application-layer handlers or flow services.
- Do not use primitive types where a strong ID or value object already exists.

## Definition of Done
- The type compiles and its `Create(...)` factory enforces all invariants.
- A private EF constructor exists only if needed.
- Domain tests cover valid creation and at least one invalid/guarded path.
- No transport or infrastructure concerns leak into the domain type.

## Questions
- Which bounded context does this type belong to?
- What are the required invariants and validation rules?
- Does it need an EF configuration, or is that a separate task?
