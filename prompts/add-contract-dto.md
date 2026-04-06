# Add a shared contract DTO in Portiforce SAA

When adding a DTO:

1. Place it under `Shared/Portiforce.SAA.Contracts`.
2. Keep it transport-only.
3. Do not add domain behavior.
4. Prefer small explicit records for request/response models.
5. Reuse existing enums from contracts where possible.
6. Add route constants to `Configuration` if the feature needs them.
7. Add an explicit mapper from application projection to DTO.

Do not:
- expose domain entities
- reference infrastructure types
- place UI-specific behavior inside contracts