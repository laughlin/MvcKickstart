using System;

namespace MvcKickstart.ViewModels.Mail
{
	public class ForgotPassword : EmailBase
	{
		public Guid Token { get; set; }
	}
}