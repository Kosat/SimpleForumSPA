using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace WebSPA
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Serilog settings
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
				.Enrich.FromLogContext()
			  .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elk_elastic:9200"))
			  {
			    IndexFormat = "serilog-{0:yyyy.MM.dd}",
			    AutoRegisterTemplate = true,
			    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
			  })
			  .Enrich.WithMachineName()
			  .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
				.CreateLogger();

			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((builderContext, config) =>
				{
					config.AddJsonFile("appsettings.endpoints.json");
					config.AddEnvironmentVariables();
				})
				.UseSerilog()
				.UseUrls("http://0.0.0.0:5001")
				.UseStartup<Startup>();
		}
	}
}
