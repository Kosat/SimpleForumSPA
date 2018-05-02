using Microsoft.AspNetCore.Identity;

namespace Api.Auth.Data
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; }
	}
}