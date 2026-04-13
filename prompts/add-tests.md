# Add tests

## Goal
Add or update xUnit + FluentAssertions tests that cover a new or changed feature.

## Where
- Domain tests: `Tests/Portiforce.SAA.Domain.Tests`
- Mirror the folder/namespace structure of the code under test.

## Steps
1. Implement the production change first (change-first workflow).
2. Find the nearest existing test file in the same feature area and mirror its structure.
3. Create or update the test class. Use the naming convention `Method_WhenCondition_ShouldExpectedResult`.
4. Cover both valid (happy-path) and invalid (guard/validation) paths.
5. Use `[Theory]` with `[InlineData]` for validation matrices where multiple inputs map to the same assertion shape.
6. Assert parameter names for argument-validation exceptions where relevant.
7. Keep assertions explicit — prefer direct `FluentAssertions` calls over custom helper indirection.
8. Run the tests to confirm they pass.

## Don'ts
- Do not write tests before the production code exists (change-first, not TDD).
- Do not introduce new test helpers or base classes unless the existing helpers are insufficient.
- Do not test infrastructure/EF details in domain test projects.
- Do not duplicate setup logic that already exists in a shared fixture or builder.

## Definition of Done
- All new or updated tests pass.
- Both valid and invalid paths are covered for the changed behavior.
- Test names follow the `Method_WhenCondition_ShouldExpectedResult` convention.
- No unrelated tests were modified or removed.

## Questions
- Which production change do these tests cover?
- Are there existing test fixtures or builders to reuse?
- Should integration-level tests be added as well, or domain-unit only?
