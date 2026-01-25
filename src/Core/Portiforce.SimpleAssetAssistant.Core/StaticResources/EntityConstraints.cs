namespace Portiforce.SimpleAssetAssistant.Core.StaticResources;

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

		/// <summary>
		/// common naming validations
		/// </summary>
		public const int NameMaxLength = 100;

		public const int ProviderSubjectMaxLength = 200;

		public const int ExternalIdMaxLength = 100;
		public const int ExternalNotesMaxLength = 255;

		public const int CodeMaxLength = 25;
	}

	public static class Domain
	{
		public static class Asset
		{
			public const int CodeMinLength = 2;
			public const int CodeMaxLength = 16;
			public const int NativeDecimalsMaxLength = 18;
			public const int NameMaxLength = 100;
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

		public static class Account
		{
			public const int AliasMinLength = 3;
			public const int AliasMaxLength = 50;
			public const int EmailMaxLength = 100;
			public const int PhoneNumberMaxLength = 15;
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
	}
}
