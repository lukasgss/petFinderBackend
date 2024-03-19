using Serilog;

namespace Api.Extensions;

public static class LoggingSetup
{
	public static void ConfigureLogging(this ConfigureHostBuilder host)
	{
		host.UseSerilog((context, loggerConfig) => { loggerConfig.ReadFrom.Configuration(context.Configuration); });
	}
}