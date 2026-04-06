# Add a new domain type in Portiforce SAA

When adding a domain entity, value object, or strongly typed primitive:

1. Place it in the correct `Backend/Core` project.
2. Keep domain concerns independent from infrastructure.
3. Use explicit validation in constructors/factories.
4. Prefer:
	- `public static Create(...)`
	- private constructor for EF Core if needed
	- domain-specific exception messages
5. Use strong IDs where identity matters.
6. Keep rule logic close to the type or in dedicated rule/guard classes.

Testing:
- add xUnit + FluentAssertions tests
- use explicit behavior-based names
- cover:
	- valid creation
	- invalid creation
	- normalization
	- equality
	- mutation rules