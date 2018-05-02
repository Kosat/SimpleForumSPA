using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace WebSPA
{
	public class Startup
	{
		private ILogger<Startup> _logger;

		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<Startup>();
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<EndpointsSettings>(Configuration.GetSection("Endpoints"));
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			var endpointsSettings = app.ApplicationServices.GetService<IOptions<EndpointsSettings>>().Value;
			Debug.Assert(endpointsSettings != null);
			Debug.Assert(!string.IsNullOrWhiteSpace(endpointsSettings.Api_Auth));
			Debug.Assert(!string.IsNullOrWhiteSpace(endpointsSettings.Spa));

			_logger.LogDebug("Settings loaded");
			_logger.LogDebug("Endpoint SPA:" + endpointsSettings.Spa);
			_logger.LogDebug("Endpoint Auth:" + endpointsSettings.Api_Auth);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseStaticFiles();
			app.UseSpaStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

			app.UseSpa(spa =>
			{
				// To learn more about options for serving an Angular SPA from ASP.NET Core,
				// see https://go.microsoft.com/fwlink/?linkid=864501
				spa.Options.SourcePath = "ClientApp";
				if (env.IsDevelopment())
				{
					string ENV_IS_IN_DOCKER = Configuration.GetValue<string>("ENV_IS_IN_DOCKER");
					if (!string.IsNullOrWhiteSpace(ENV_IS_IN_DOCKER) 
						&& string.Compare(bool.TrueString, ENV_IS_IN_DOCKER, ignoreCase: true) == 0)
					{
						_logger.LogInformation("Starting: Executing npm script 'docker:start'");
						spa.UseAngularCliServer(npmScript: "docker:start");
					}
					else
					{
						_logger.LogInformation("Starting: Executing npm script 'vs:start'");
						spa.UseAngularCliServer(npmScript: "vs:start");
					}
                }
			});


		}
	}
}