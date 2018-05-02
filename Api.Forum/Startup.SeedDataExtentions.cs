using System;
using System.Data.SqlClient;
using Api.Forum.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Api.Forum
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
				var context = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
				context.Database.Migrate();
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