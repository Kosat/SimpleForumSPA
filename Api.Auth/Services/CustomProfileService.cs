﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Auth.Data;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace Api.Auth.Services
{
	public class CustomProfileService : IProfileService
	{
		private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
		private readonly UserManager<ApplicationUser> _userManager;

		public CustomProfileService(UserManager<ApplicationUser> userManager,
			IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
		{
			_userManager = userManager;
			_claimsFactory = claimsFactory;
		}

		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var sub = context.Subject.GetSubjectId();

			var user = await _userManager.FindByIdAsync(sub);
			var userRoles = await _userManager.GetRolesAsync(user);
			var principal = await _claimsFactory.CreateAsync(user);

			var claims = principal.Claims.ToList();

			claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

			claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));

			foreach (var role in userRoles)
			{
				claims.Add(new Claim(JwtClaimTypes.Role, role));
			}

			claims.Add(new Claim(JwtClaimTypes.Scope, "api.forum"));
			claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
			context.IssuedClaims = claims;
		}

		public async Task IsActiveAsync(IsActiveContext context)
		{
			var sub = context.Subject.GetSubjectId();
			var user = await _userManager.FindByIdAsync(sub);
			context.IsActive = user != null;
		}
	}
}