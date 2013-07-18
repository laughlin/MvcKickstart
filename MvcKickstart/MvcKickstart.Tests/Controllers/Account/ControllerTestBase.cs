using System.Net.Mail;
using System.Text;
using ActionMailer.Net;
using ActionMailer.Net.Mvc;
using Moq;
using KickstartTemplate.Controllers;
using KickstartTemplate.Models.Users;
using KickstartTemplate.Services;
using KickstartTemplate.Tests.Utilities;

namespace KickstartTemplate.Tests.Controllers.Account
{
	public abstract class ControllerTestBase : TestBase
	{
		protected Mock<IUserService> UserService { get; set; }
		protected Mock<IUserAuthenticationService> AuthenticationService { get; set; }
		protected Mock<IMailController> MailController { get; set; }
		protected AccountController Controller { get; set; }
		protected User User { get; private set; }

		public override void SetupFixture()
		{
			base.SetupFixture();

			User = Generator.SetupUser();
		}

		public override void Setup()
		{
			base.Setup();

			MailController = new Mock<IMailController>();
			var emailResult = new EmailResult(new Mock<IMailInterceptor>().Object, new Mock<IMailSender>().Object, new MailMessage(), "", "", Encoding.Unicode, false);
			MailController.Setup(x => x.ForgotPassword(It.IsAny<ViewModels.Mail.ForgotPassword>())).Returns(emailResult);
			MailController.Setup(x => x.Welcome(It.IsAny<ViewModels.Mail.Welcome>())).Returns(emailResult);
			UserService = new Mock<IUserService>();
			AuthenticationService = new Mock<IUserAuthenticationService>();
			AuthenticationService.Setup(x => x.ReservedUsernames).Returns(new[] { "admin" });

			Controller = new AccountController(Db, Metrics, Cache, MailController.Object, UserService.Object, AuthenticationService.Object);
			ControllerUtilities.SetupControllerContext(Controller);
		}
	}
}
