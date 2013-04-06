using System.Configuration;
using ActionMailer.Net.Mvc;
using MvcKickstart.ViewModels.Mail;

namespace MvcKickstart.Controllers
{
	public interface IMailController
	{
		EmailResult Welcome(Welcome model);
		EmailResult ForgotPassword(ForgotPassword model);
	}

	public class MailController : MailerBase, IMailController
	{
		public EmailResult Welcome(Welcome model)
		{
			SetToAndFromValues(model);

			Subject = "Welcome to MvcKickstart";
			return Email("Welcome", model);
		}
		public EmailResult ForgotPassword(ForgotPassword model)
		{
			SetToAndFromValues(model);

			Subject = "[MvcKickstart] Forgot password";
			return Email("ForgotPassword", model);
		}

		private void SetToAndFromValues(EmailBase model)
		{
			To.Add(model.To);
			From = model.From ?? ConfigurationManager.AppSettings["Email:Support"];
		}
	}
}