using System;

namespace KickstartTemplate.ViewModels.Mail
{
	public class ForgotPassword : EmailBase
	{
		public Guid Token { get; set; }
	}
}