using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Api.Auth.Data;
using Api.Auth.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Auth
{
	public class Startup
	{
		private readonly ILoggerFactory _loggerFactory;
		private IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<BuiltInUsersSettings>(Configuration.GetSection("BuiltInUsers"));
			services.Configure<EndpointsSettings>(Configuration.GetSection("Endpoints"));

			services.AddDbContext<ApplicationDbContext>(
			  options => options.UseSqlServer(
					Configuration.GetValue<string>("DbConnectionString"),
					x => x.MigrationsAssembly("Api.Auth")));

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			var connectionString = Configuration.GetValue<string>("DbConnectionString");
			var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
				{
					options.User.RequireUniqueEmail = false;
					options.Password.RequireDigit = false;
					options.Password.RequiredLength = 3;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireLowercase = false;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddTransient<IProfileService, CustomProfileService>();
			services.AddTransient<RedirectService, RedirectService>();

			var cors = new DefaultCorsPolicyService(_loggerFactory.CreateLogger<DefaultCorsPolicyService>())
			{
				AllowedOrigins = { Configuration.GetValue<string>("endpoints:Spa")}
			};
			services.AddSingleton<ICorsPolicyService>(cors);

			services.AddIdentityServer(x => x.IssuerUri = Configuration.GetValue<string>("endpoints:Api_Auth"))
				.AddConfigurationStore(options =>
					options.ConfigureDbContext =
						builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly))
				)
				.AddOperationalStore(options =>
				{
					options.ConfigureDbContext =
						builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
					options.EnableTokenCleanup = true;
					options.TokenCleanupInterval = 30;
				})
				.AddDeveloperSigningCredential()
			  //.AddSigningCredential(new X509Certificate2(Path.Combine(".", "IdentityServer4Auth.pfx")))
			  .AddAspNetIdentity<ApplicationUser>()
			  .AddProfileService<CustomProfileService>();

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials());
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseHsts();
			}

			app.EnsureSeedData();
			app.UseCors("CorsPolicy");
			app.UseStaticFiles();
			app.UseIdentityServer();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Account}/{action=Register}/{id?}");
			});
		}

	}
}
