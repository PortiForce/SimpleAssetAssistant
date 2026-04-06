# Add tests in Portiforce SAA

Testing conventions:

- framework: xUnit
- assertions: FluentAssertions
- naming:
	- `Method_WhenCondition_ShouldExpectedResult`

Guidelines:
1. Follow the nearest test file in `Portiforce.SAA.Domain.Tests`.
2. Prefer `[Theory]` for repeated validation scenarios.
3. Assert exact parameter names for argument exceptions when the production code uses them.
4. Keep tests deterministic and domain-focused.
5. Avoid unnecessary mocks in domain tests.
6. Cover both success and failure paths.

Patterns commonly used:
- `Func<T>` or `Action` for exception assertions
- `.Throw<TException>()`
- `.Which.ParamName.Should().Be(...)`
- equality/hash code checks for value objects