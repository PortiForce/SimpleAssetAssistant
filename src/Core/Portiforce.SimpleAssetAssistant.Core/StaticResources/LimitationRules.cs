namespace Portiforce.SimpleAssetAssistant.Core.StaticResources;

public static class LimitationRules
{
	public static class Lengths
	{
		// Length limitations for various string fields

		/// <summary>
		/// common naming validations
		/// </summary>
		public const int NameMaxLength = 100;

		public const int ProviderSubjectMaxLength = 200;

		public static class Asset
		{
			public const int CodeMinLength = 2;
			public const int CodeMaxLength = 16;
		}

		public static class Tenant
		{
			public const int MaxRowsPerFile = 10_000;
			public const int MaxFileSizeMb = 10;

			public const int MinNameLength = 3;
			public const int MaxNameLength = 3;
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
			public const int MaxAliasLength = 3;
		}
	}
}
