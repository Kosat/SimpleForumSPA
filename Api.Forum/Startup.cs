using System;
using System.Collections.Generic;
using System.Diagnostics;
using Api.Forum.Data;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Api.Forum
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		public void ConfigureServices(IServiceCollection services)
        {
	        services.Configure<EndpointsSettings>(Configuration.GetSection("Endpoints"));

			services.AddDbContext<ForumDbContext>(
		        options => options.UseSqlServer(
					Configuration.GetValue<string>("DbConnectionString"),
					x => x.MigrationsAssembly("Api.Forum")));

			services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
		        .AddIdentityServerAuthentication(options =>
				{
					options.Authority = Configuration.GetValue<string>("endpoints:Api_Auth");

					options.RequireHttpsMetadata = false;
					options.ApiName = "api.forum";
		        });

	        services.AddAuthorization(options =>
	        {
		        options.AddPolicy("role:admin", policyAdmin =>
		        {
			        policyAdmin.RequireClaim("role", "admin");
		        });
		        options.AddPolicy("role:user", policyUser =>
		        {
			        policyUser.RequireClaim("role", "user");
		        });
		        options.AddPolicy("role:moderator", policyUser =>
		        {
			        policyUser.RequireClaim("role", "moderator");
		        });
				options.AddPolicy("scope:api.forum", policyUser =>
		        {
			        policyUser.RequireClaim("scope", "api.forum");
		        });
	        });

			var defaultPolicy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.RequireClaim("scope", "api.forum")
				.Build();

			services.AddMvc(options =>
	        {
		       options.Filters.Add(new AuthorizeFilter(defaultPolicy));
	        });

	        services.AddSwaggerGen(options =>
	        {
		        options.DescribeAllEnumsAsStrings();
				var security = new Dictionary<string, IEnumerable<string>>
				{
					{"Bearer", new string[] { }},
				};
				options.AddSecurityDefinition("Bearer", new ApiKeyScheme
				{
					Description = "Authorization: Bearer <token>",
					Name = "Authorization",
					In = "header",
					Type = "apiKey"
				});
				options.AddSecurityRequirement(security);

				options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
		        {
			        Title = "Api.Forum",
			        Version = "v1",
			        Description = "SimpleForum backend",
		        });
	        });

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials());
			});

		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// </summary>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			var endpointsSettings = app.ApplicationServices.GetService<IOptions<EndpointsSettings>>().Value;
			Debug.Assert(endpointsSettings!=null);
			Debug.Assert(!string.IsNullOrWhiteSpace(endpointsSettings.Api_Auth));
			Debug.Assert(!string.IsNullOrWhiteSpace(endpointsSettings.Spa));

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.EnsureSeedData();

			app.UseCors("CorsPolicy");
			app.UseMiddleware<SerilogMiddleware>();
			app.UseAuthentication();
			app.UseMvc();

	        app.UseSwagger()
		        .UseSwaggerUI(c =>
		        {
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api.Forum v1");
		        });
		}
    }
}
