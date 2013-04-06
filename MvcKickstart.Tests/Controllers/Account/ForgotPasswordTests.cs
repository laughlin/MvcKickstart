using System.Web.Mvc;
using FizzWare.NBuilder.Generators;
using Moq;
using MvcKickstart.Tests.Utilities;
using NUnit.Framework;
using Should.Fluent;
using MvcKickstart.ViewModels.Account;

namespace MvcKickstart.Tests.Controllers.Account
{
	public class ForgotPasswordTests : ControllerTestBase
	{
		[Test]
		public void GivenAuthenticatedGetRequest_ReturnsView()
		{
			ControllerUtilities.SetupControllerContext(Controller, User);
			var result = Controller.ForgotPassword() as ViewResult;
			result.Should().Not.Be.Null();
		}
		[Test]
		public void GivenUnauthenticatedGetRequest_ReturnsView()
		{
			ControllerUtilities.SetupControllerContext(Controller);

			var result = Controller.ForgotPassword() as ViewResult;
			result.Should().Not.Be.Null();
		}
		[Test]
		public void GivenInvalidPostRequest_ReturnsView()
		{
			var model = new ForgotPassword
				{
					Email = GetRandom.String(20)
				};
			Controller.ModelState.AddModelError("Email", "Email address is invalid");

			var result = Controller.ForgotPassword(model) as ViewResult;
			result.Should().Not.Be.Null();
			result.ViewName.Should().Not.Equal("ForgotPasswordConfirmation");
		}
		[Test]
		public void GivenValidPostRequest_AndNoUserWithEmail_ReturnsView()
		{
			var model = new ForgotPassword
				{
					Email = GetRandom.Email()
				};

			var result = Controller.ForgotPassword(model) as ViewResult;
			result.Should().Not.Be.Null();
			result.ViewName.Should().Not.Equal("ForgotPasswordConfirmation");
			Controller.ModelState.Count.Should().Equal(1);
		}
		[Test]
		public void GivenValidPostRequest_ReturnsConfirmationView()
		{
			var model = new ForgotPassword
				{
					Email = User.Email
				};

			var result = Controller.ForgotPassword(model) as ViewResult;
			result.Should().Not.Be.Null();
			result.ViewName.Should().Equal("ForgotPasswordConfirmation");
		}
		[Test]
		public void GivenValidPostRequest_SendsEmail()
		{
			var model = new ForgotPassword
				{
					Email = User.Email
				};

			Controller.ForgotPassword(model);

			MailController.Verify(x => x.ForgotPassword(It.Is<ViewModels.Mail.ForgotPassword>(m => m.To == User.Email)), Times.Once());
		}
	}
}
