using System.ComponentModel.DataAnnotations;

namespace KickstartTemplate.ViewModels.Account
{
	public class Login
	{
		[Display(Name= "Username or Email")]
		[Required(ErrorMessage = "{0} is required")]
		[StringLength(256, ErrorMessage = "{0} must be no more than {1} characters")]
		public string Username { get; set; }

		[Display(Name = "Password")]
		[Required(ErrorMessage = "{0} is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
        [UIHint("YesNoRadioList")]
		[Display(Name = "Remember Me")]
		public bool RememberMe { get; set; }

		public string ReturnUrl { get; set; }
	}
}