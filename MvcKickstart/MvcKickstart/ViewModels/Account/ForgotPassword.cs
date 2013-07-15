using System.ComponentModel.DataAnnotations;

namespace MvcKickstart.ViewModels.Account
{
	public class ForgotPassword
	{
		[Display(Name = "Email")]
		[Required(ErrorMessage = "{0} is required")]
		[EmailAddress(ErrorMessage = "{0} is not a valid email address")]
		public string Email { get; set; }
	}
}