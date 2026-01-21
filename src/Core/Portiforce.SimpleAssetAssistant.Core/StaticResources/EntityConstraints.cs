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

		public const int EmailAddressLength = 100;

		/// <summary>
		/// common naming validations
		/// </summary>
		public const int NameMaxLength = 100;

		public const int ProviderSubjectMaxLength = 200;
	}

	public static class Domain
	{
		public static class Asset
		{
			public const int CodeMinLength = 2;
			public const int CodeMaxLength = 16;
			public const int NativeDecimalsMaxLength = 18;
		}

		public static class Tenant
		{
			public const int MinNameLength = 3;
			public const int MaxCodeLength = 25;
			public const int MaxNameLength = 100;
			public const int MaxBrandLength = 100;
			public const int MaxPublicDomainLength = 50;

			public const int MaxRowsPerFile = 100_000;
			public const int MaxFileSizeMb = 5;
		}

		public static class User
		{
			public const int FullNameMaxLength = 100;
			public const int EmailMaxLength = 255;
			public const int PhoneNumberMaxLength = 15;
		}

		public static class Account
		{
			public const int MinAliasLength = 3;
			public const int MaxAliasLength = 50;
		}

		public static class Activity
		{
			
		}

		public static class Platform
		{
			public const int MinNameLength = 3;
			public const int MaxNameLength = 100;
		}
	}
}
