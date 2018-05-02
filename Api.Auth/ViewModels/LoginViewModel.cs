using System.ComponentModel.DataAnnotations;

namespace Api.Auth.ViewModels
{
	public class LoginViewModel
	{
		[Display(Name = "Email", Prompt = "Email")]
		[EmailAddress]
		[Required(ErrorMessage = "Email required")]
		public string Email { get; set; }

		[Display(Name = "Password", Prompt = "Password")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Password required")]
		public string Password { get; set; }
		public bool RememberLogin { get; set; }
		public string ReturnUrl { get; set; }
	}
}