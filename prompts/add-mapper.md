# Add a mapper in Portiforce SAA

When adding mapping logic:

1. Place it in an existing mapper class if it fits.
2. Otherwise add a new static mapper in the relevant layer.
3. Prefer extension methods.
4. Keep mapping explicit and one-way per use case.
5. Translate enums with switch expressions.
6. Map projections to DTOs in web/shared boundaries.
7. Do not hide complex business logic inside mapping.

Prefer existing examples:
- `InviteMapper`
- `AccountMapper`