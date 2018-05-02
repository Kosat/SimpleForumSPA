using System;
using System.Data.SqlClient;
using System.Linq;
using Api.Auth.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Api.Auth
{
	public static class SeedDataExtentions
	{
		public static async void EnsureSeedData(this IApplicationBuilder app)
		{
			var loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
			if (loggerFactory == null)
				throw new ArgumentNullException(nameof(loggerFactory));

			var logger = loggerFactory.CreateLogger("Startup.SeedData");

			var policy = CreatePolicy(logger, "SeedDataPolicy");

			await policy.ExecuteAsync(async () =>
			{
				EnsureSeedDataInternal(app, logger);
			});
		}

		private static void EnsureSeedDataInternal(IApplicationBuilder app, ILogger logger)
		{
			var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

			logger.LogInformation("Seeding database...");

			using (var scope = scopeFactory.CreateScope())
			{
					// Create ASP MVC Identity database schema
					scope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();

					// Create PersistedGrant database schema
					scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

					// Create Configuration database schema
					EnsureSeedData_ConfigurationDb(scope, logger);

				EndureSeedData_UserRoles(scope, logger);
					EndureSeedData_BuildInUsers(scope, logger);
				}

			logger.LogInformation("Done seeding database.");
		}

		private static void EndureSeedData_UserRoles(IServiceScope scope, ILogger logger)
		{
			string[] roleNames = {"user", "moderator", "admin"};
			var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

			logger.LogInformation("Creating user roles...");

			foreach (var roleName in roleNames)
			{
				var roleExist = roleManager.RoleExistsAsync(roleName).Result;
				if (!roleExist)
				{
					var roleResult = roleManager.CreateAsync(new IdentityRole(roleName)).Result;

					if (roleResult != IdentityResult.Success)
						throw new Exception($"Failed to create role '{roleName}'");
				}
			}
		}

		private static void EndureSeedData_BuildInUsers(IServiceScope scope, ILogger logger)
		{
			//Create the default Admin account if not exists
			var usersSettings = scope.ServiceProvider.GetService<IOptions<BuiltInUsersSettings>>();
			var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
			var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

			foreach (var builtInUser in usersSettings.Value.Users)
			{
				string adminUserEmail = builtInUser.Email;
				string adminPass = builtInUser.Password;
				string adminUserRole = builtInUser.Role;

				logger.LogInformation("Creating build-in users...");

				if (!applicationDbContext.Users.Any(r => r.UserName == adminUserEmail))
				{
					ApplicationUser user = new ApplicationUser
					{
						UserName = adminUserEmail,
						Email = adminUserEmail,
						EmailConfirmed = true
					};
					userManager.CreateAsync(user, password: adminPass).Wait();
					string[] adminRoles = adminUserRole.Split(' ');
					foreach (var role in adminRoles)
					{
						userManager.AddToRoleAsync(user, role).Wait();
					}

					logger.LogInformation($"Created admin user '{user.Name}'");
				}
			}
		}

		private static void EnsureSeedData_ConfigurationDb(IServiceScope scope, ILogger logger)
		{
			var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
			configurationDbContext.Database.Migrate();

			if (!configurationDbContext.Clients.Any())
			{
				logger.LogInformation("Clients being populated");
				var endpointsSettings = scope.ServiceProvider.GetService<IOptions<EndpointsSettings>>().Value;
				foreach (var client in IdentityServerSettings.GetOpenIdClients(endpointsSettings).ToList())
				{
					configurationDbContext.Clients.Add(client.ToEntity());
				}
				configurationDbContext.SaveChanges();
			}
			else
			{
				logger.LogInformation("Clients already populated");
			}

			if (!configurationDbContext.IdentityResources.Any())
			{
				logger.LogInformation("IdentityResources being populated");
				foreach (var resource in IdentityServerSettings.GetIdentityResources().ToList())
				{
					configurationDbContext.IdentityResources.Add(resource.ToEntity());
				}
				configurationDbContext.SaveChanges();
			}
			else
			{
				logger.LogInformation("IdentityResources already populated");
			}

			if (!configurationDbContext.ApiResources.Any())
			{
				logger.LogInformation("ApiResources being populated");
				foreach (var resource in IdentityServerSettings.GetApiResources().ToList())
				{
					configurationDbContext.ApiResources.Add(resource.ToEntity());
				}
				configurationDbContext.SaveChanges();
			}
			else
			{
				logger.LogInformation("ApiResources already populated");
			}

		}

		private static Policy CreatePolicy(ILogger logger, string prefix)
		{
			return Policy.Handle<SqlException>().
				WaitAndRetryForeverAsync(
					sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
					onRetry: (exception, timeSpan) =>
					{
						logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected.");
					}
				);
		}
	}
}