using Portiforce.SAA.Core.Exceptions;
using Portiforce.SAA.Core.Identity.Enums;
using Portiforce.SAA.Core.Models;
using Portiforce.SAA.Core.Primitives;
using Portiforce.SAA.Core.Primitives.Ids;

namespace Portiforce.SAA.Core.Identity.Models.Auth;

/// <summary>
///     Represents a tenant-scoped identifier that is reserved by and belongs to an account.
///     Answers to the question: Which identifiers are already owned by this account inside this tenant?
///     Part of the account ownership / uniqueness / onboarding protection area.
/// </summary>
public sealed class AccountIdentifier : Entity<AccountIdentifierId>
{
	private AccountIdentifier(
		AccountIdentifierId id,
		TenantId tenantId,
		AccountId accountId,
		AccountIdentifierKind kind,
		string value,
		bool isVerified,
		bool isPrimary)
		: base(id)
	{
		if (id.IsEmpty)
		{
			throw new DomainValidationException("Identifier id must be defined.");
		}

		if (tenantId.IsEmpty)
		{
			throw new DomainValidationException("TenantId must be defined.");
		}

		if (accountId.IsEmpty)
		{
			throw new DomainValidationException("AccountId must be defined.");
		}

		string normalizedValue = Normalize(kind, value);

		if (string.IsNullOrWhiteSpace(normalizedValue))
		{
			throw new DomainValidationException("Identifier value is required.");
		}

		this.TenantId = tenantId;
		this.AccountId = accountId;
		this.Kind = kind;
		this.Value = normalizedValue;
		this.IsVerified = isVerified;
		this.IsPrimary = isPrimary;
	}

	// Private Empty Constructor for EF Core
	private AccountIdentifier()
	{
	}

	public TenantId TenantId { get; init; }

	public AccountId AccountId { get; init; }

	public AccountIdentifierKind Kind { get; init; }

	public string Value { get; init; }

	public bool IsVerified { get; private set; }

	public bool IsPrimary { get; private set; }

	public static AccountIdentifier Create(
		TenantId tenantId,
		AccountId accountId,
		AccountIdentifierKind kind,
		string value,
		bool isVerified,
		bool isPrimary,
		AccountIdentifierId id = default)
	{
		return new AccountIdentifier(
			id.IsEmpty ? AccountIdentifierId.New() : id,
			tenantId,
			accountId,
			kind,
			value,
			isVerified,
			isPrimary);
	}

	private static string Normalize(AccountIdentifierKind kind, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}

		value = value.Trim();

		return kind switch
		{
			AccountIdentifierKind.Email => Email.Create(value).ToString(),
			AccountIdentifierKind.Phone => PhoneNumber.Create(value).ToString(),
			AccountIdentifierKind.TelegramUserId => value.Trim(),
			AccountIdentifierKind.TelegramUserName => value.Trim(),
			_ => value.Trim()
		};
	}
}