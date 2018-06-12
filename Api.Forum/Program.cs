using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace Api.Forum
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

			BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
	            .ConfigureAppConfiguration((builderContext, config) =>
	            {
		            config.AddEnvironmentVariables();
	            })
	            .UseSerilog()
				.UseStartup<Startup>()
				.UseUrls("http://0.0.0.0:5003")
				.Build();
    }
}
