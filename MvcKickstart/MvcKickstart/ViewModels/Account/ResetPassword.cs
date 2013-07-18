using System;
using System.ComponentModel.DataAnnotations;
using KickstartTemplate.Infrastructure.Attributes;
using KickstartTemplate.Models.Users;

namespace KickstartTemplate.ViewModels.Account
{
	public class ResetPassword
	{
		public Guid Token { get; set; }
		public PasswordRetrieval Data { get; set; }

		[Display(Name = "New Password")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "{0} is required")]
		[StringLength(1000, ErrorMessage = "{0} must be at least {2} characters", MinimumLength = 8)]
		[PasswordStrength]
		public string Password { get; set; }

		[Display(Name = "Confirm Password")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "{0} is required")]
		[Compare("Password", ErrorMessage = "Passwords do not match")]
		public string PasswordConfirm { get; set; }
	}
}