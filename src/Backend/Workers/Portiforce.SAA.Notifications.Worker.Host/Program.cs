using Portiforce.SAA.Application;
using Portiforce.SAA.Application.Interfaces.Common.Time;
using Portiforce.SAA.Infrastructure;
using Portiforce.SAA.Infrastructure.EF;
using Portiforce.SAA.Infrastructure.Invite;
using Portiforce.SAA.Infrastructure.Services.Time;

namespace Portiforce.SAA.Notifications.Worker.Host;

public class Program
{
	public static void Main(string[] args)
	{
		HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

		if (builder.Environment.IsDevelopment())
		{
			_ = builder.Configuration.AddUserSecrets<Program>(true);
		}

		ValidateRequiredConnectionString(builder.Configuration);

		_ = builder.Services
			.AddOptions<InviteNotificationWorkerOptions>()
			.Bind(builder.Configuration.GetSection(InviteNotificationWorkerOptions.SectionName))
			.Validate(InviteNotificationWorkerOptions.IsValid, "Invite notification worker options are invalid.")
			.ValidateOnStart();

		_ = builder.Services.AddApplication();
		_ = builder.Services.AddInfrastructure(builder.Configuration);
		_ = builder.Services.AddEfInfrastructure(builder.Configuration);
		_ = builder.Services.AddNotificationsInfrastructure(builder.Configuration);

		_ = builder.Services.AddOptions<InviteEmailOptions>()
			.Validate(
				options =>
					IsConfiguredValue(options.Host) &&
					options.Port > 0 &&
					IsConfiguredValue(options.FromEmail) &&
					HasValidOptionalSmtpCredentials(options) &&
					!ContainsPlaceholder(options.SubjectPrefix),
				"InviteEmailOptions must define Host, Port, FromEmail, and valid optional SMTP credentials.")
			.ValidateOnStart();

		_ = builder.Services.AddSingleton<IClock, SystemClock>();
		_ = builder.Services.AddHostedService<InviteNotificationBackgroundService>();

		IHost host = builder.Build();
		host.Run();
	}

	private static void ValidateRequiredConnectionString(IConfiguration configuration)
	{
		string? connectionString = configuration.GetConnectionString("AssetAssistantSQLDb");

		if (!IsConfiguredValue(connectionString))
		{
			throw new InvalidOperationException(
				"ConnectionStrings:AssetAssistantSQLDb must be configured for the invite notification worker.");
		}
	}

	private static bool IsConfiguredValue(string? value) =>
		!string.IsNullOrWhiteSpace(value) &&
		!ContainsPlaceholder(value);

	private static bool HasValidOptionalSmtpCredentials(InviteEmailOptions options)
	{
		bool hasUserName = IsConfiguredValue(options.UserName);
		bool hasPassword = IsConfiguredValue(options.Password);
		bool hasNoCredentials =
			string.IsNullOrWhiteSpace(options.UserName) &&
			string.IsNullOrWhiteSpace(options.Password);

		return hasNoCredentials || (hasUserName && hasPassword);
	}

	private static bool ContainsPlaceholder(string? value) =>
		value?.Contains("{from-configs}", StringComparison.OrdinalIgnoreCase) == true;
}