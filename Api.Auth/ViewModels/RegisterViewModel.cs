using System.ComponentModel.DataAnnotations;

namespace Api.Auth.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email", Prompt = "Email")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password", Prompt = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "ConfirmPassword", Prompt = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Role { get; set; }

		public string ReturnUrl { get; set; }
	}
}
