namespace Portiforce.SAA.Core.StaticResources;

public static class EntityConstraints
{
	// Length limitations for various string fields
	public static class CommonSettings
	{
		public const int ExtraShortStringLength = 10;
		public const int ShortStringLength = 50;
		public const int MediumStringLength = 100;
		public const int LongStringLength = 255;
		public const int ExtraLongStringLength = 1000;

		public const int EmailAddressDefaultLength = 100;
		public const int EmailAddressMaxLength = 255;

		public const int PhoneNumberMinLength = 9;
		public const int PhoneNumberMaxLength = 16;

		/// <summary>
		///     Common naming validations
		/// </summary>
		public const int NameMaxLength = 100;

		public const int ProviderSubjectMaxLength = 256;

		public const int ExternalIdMaxLength = 256;
		public const int ExternalNotesMaxLength = 256;

		public const int CodeMaxLength = 25;
		public const int GuidLikeRowMaxLength = 36;

		public const int FiatCurrencyLength = 3;
	}

	public static class Domain
	{
		public static class Asset
		{
			public const int CodeMinLength = 1;
			public const int CodeMaxLength = 16;
			public const int NameMaxLength = 100;

			public const int NativeDecimalsZeroLength = 0;
			public const int NativeDecimalsFiatLength = 2;
			public const int NativeDecimalsMinLength = 6;
			public const int NativeDecimalsAvgLength = 12;
			public const int NativeDecimalsMaxLength = 18;
		}

		public static class Tenant
		{
			public const int NameMinLength = 3;
			public const int NameMaxLength = 100;
			public const int BrandMinLength = 100;
			public const int PublicDomainMaxLength = 50;

			public const int MaxRowsPerFile = 100_000;
			public const int MaxFileSizeMb = 5;
		}

		public static class Invite
		{
			public const int AliasMinLength = 3;
			public const int AliasMaxLength = 100;
		}

		public static class Account
		{
			public const int AliasMinLength = 3;
			public const int AliasMaxLength = 255;
		}

		public static class Platform
		{
			public const int NameMinLength = 3;
			public const int NameMaxLength = 100;
		}

		public static class PlatformAccount
		{
			public const int NameMinLength = 3;
			public const int NameMaxLength = 100;
		}

		public static class Activity
		{
			public const int FuturesInstrumentKeyLength = 100;
		}

		public static class ActivityLeg
		{
			public const int InstrumentKeyMaxLength = 100;
		}

		public static class AuthSessionToken
		{
			public const int IpAddressMaxLength = 45;
			public const int UserAgentMaxLength = 256;
			public const int UserAgentFingerprint = 32;
		}

		public static class InfrastructureMessage
		{
			public const int PublicReferenceMaxLength = 32;
			public const int TypeMaxLength = 300;
			public const int SourceMaxLength = 200;
			public const int RequestPathMaxLength = 2_048;
			public const int HttpMethodMaxLength = 16;
			public const int RemoteIpAddressMaxLength = 64;
			public const int UserAgentMaxLength = 512;
			public const int IdempotencyKeyMaxLength = 500;
			public const int LastErrorMaxLength = 4_000;
		}
	}
}
