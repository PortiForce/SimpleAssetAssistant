# Add an EF repository

## Goal
Add a new EF Core repository (or extend an existing one) for persistence access.

## Where
- Repository: `Backend/Infra/Portiforce.SAA.Infrastructure.EF/Repositories`
- Interface: `Backend/App/.../Interfaces/Persistence`

## Steps
1. Add or reuse the repository interface in `Backend/App/.../Interfaces/Persistence`.
2. Create the repository class in `Backend/Infra/Portiforce.SAA.Infrastructure.EF/Repositories`.
3. Keep repository methods persistence-focused: queries, saves, deletes.
4. Return projections or aggregates expected by the application layer.
5. Keep mapping from EF results explicit.
6. Keep tenant filtering and identity filtering explicit where required.
7. Check for a matching EF entity configuration; add one if missing.
8. Register the repository in DI.
9. Follow existing repository naming and folder patterns.

## Don'ts
- Do not move business rules into repository code.
- Do not return raw EF entities to callers when a projection is expected.
- Do not bypass tenant or identity filtering.
- Do not add a new repository when an existing one covers the same aggregate.

## Definition of Done
- Repository and interface compile and are registered in DI.
- Tenant/identity filtering is explicit.
- EF configuration exists for the entity.
- Application handler can inject and use the repository.

## Questions
- Which aggregate or entity does this repository manage?
- Does a repository for this aggregate already exist?
- What tenant or identity filtering rules apply?