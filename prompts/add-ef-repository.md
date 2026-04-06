# Add an EF repository in Portiforce SAA

When adding an EF repository:

1. Place it in `Backend/Infra/Portiforce.SAA.Infrastructure.EF/Repositories`.
2. Add or reuse the corresponding interface in `Backend/App/.../Interfaces/Persistence`.
3. Keep repository methods persistence-focused.
4. Return projections or aggregates expected by the application layer.
5. Keep mapping from EF results explicit.
6. Do not move business rules into repository code.
7. Keep tenant filtering and identity filtering explicit where required.

Also check:
- matching EF configuration
- DI registration
- existing repository naming and folder patterns