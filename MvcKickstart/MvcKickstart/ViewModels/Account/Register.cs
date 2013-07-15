using System.ComponentModel.DataAnnotations;
using MvcKickstart.Infrastructure.Attributes;

namespace MvcKickstart.ViewModels.Account
{
	public class Register
	{
		[Display(Name = "Email", Description = "Please provide a valid address, we need a way to contact you! We don't share this information.")]
		[Required(ErrorMessage = "{0} is required")]
		[EmailAddress(ErrorMessage = "{0} is not a valid email address")]
		[StringLength(200, ErrorMessage = "{0} cannot be more than {1} characters")]
		public string Email { get; set; }

		[Required(ErrorMessage = "{0} is required")]
		[StringLength(50, ErrorMessage = "{0} cannot be more than {1} characters")]
		public string Username { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters", MinimumLength = 1)]
        public string Name { get; set; }

		[Display(Name = "Password", Description = "One number and one uppercase letter required.")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "{0} is required")]
		[StringLength(5000, ErrorMessage = "{0} must be at least {2} characters", MinimumLength = 8)]
		[PasswordStrength]
		public string Password { get; set; }

		public string ReturnUrl { get; set; }
	}
}