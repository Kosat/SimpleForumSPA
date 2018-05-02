using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Api.Auth
{
	public class IdentityServerSettings
	{
		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile()
			};
		}

		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource>
			{
				new ApiResource("api.forum","Forum Api resource.")
			};
		}

		public static IEnumerable<Client> GetOpenIdClients(EndpointsSettings endpoints)
		{
			return new List<Client>
			{
				new Client
				{
					ClientId = "js",
					ClientName = "SPA Client",
					AllowedGrantTypes = GrantTypes.Implicit,
					AllowAccessTokensViaBrowser = true,
					RequireConsent = false,
					RedirectUris = {
						$"{endpoints.Spa_External}/",//Trailing slash in the end
						$"{endpoints.Spa_External}/auth.html",
						$"{endpoints.Spa_External}/silent-renew.html",
						$"{endpoints.Spa}/",//Trailing slash in the end
						$"{endpoints.Spa}/auth.html",
						$"{endpoints.Spa}/silent-renew.html"},
					PostLogoutRedirectUris = {$"{endpoints.Spa}/", $"{endpoints.Spa_External}/"}, //Trailing slash in the end
					AllowedCorsOrigins = {endpoints.Spa, endpoints.Spa_External}, //No trailing slash in the end
					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"api.forum",
						"role"
					}
				}
			};
		}
	}
}