using System.Web.Mvc;
using FizzWare.NBuilder.Generators;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Infrastructure.Extensions;
using Moq;
using KickstartTemplate.Tests.Utilities;
using KickstartTemplate.ViewModels.Account;
using MvcKickstart.Infrastructure;
using NUnit.Framework;
using Should.Fluent;

namespace KickstartTemplate.Tests.Controllers.Account
{
	public class RegisterTests : ControllerTestBase
	{
		[Test]
		public void GivenUnauthenticatedRequest_ReturnsRegisterView()
		{
			var result = Controller.Register("") as ViewResult;
			result.Should().Not.Be.Null();
		}
		[Test]
		public void GivenAuthenticatedRequest_ReturnsHomePage()
		{
			ControllerUtilities.SetupControllerContext(Controller, User);

			var result = Controller.Register("") as RedirectToRouteResult;
			result.Should().Not.Be.Null();
		}

		[Test]
		public void GivenValidModelState_ReturnsRedirect()
		{
			var model = new Register
			{
				Username = GetRandom.String(20),
				Email = GetRandom.Email(),
				Password = "password1",
				ReturnUrl = "http://google.com"
			};

			var result = Controller.Register(model) as RedirectResult;
			result.Should().Not.Be.Null();
			Controller.TempData[ViewDataConstants.Notification].Should().Not.Be.Null();
			result.Url.Should().Equal(Controller.Url.Home().Index());
		}

		[Test]
		public void GivenInvalidModelState_ReturnsRegisterView()
		{
			var model = new Register
			{
				Username = GetRandom.String(20),
				Email = GetRandom.Email(),
				Password = "password1"
			};
			Controller.ModelState.AddModelError("Fake error", "error");

			var result = Controller.Register(model) as ViewResult;
			result.Should().Not.Be.Null();

			result.Model.Should().Be.OfType<Register>();
			var typedModel = result.Model as Register;
			typedModel.Email.Should().Equal(model.Email);
			typedModel.Password.Should().Be.NullOrEmpty();
			typedModel.ReturnUrl.Should().Equal(model.ReturnUrl);
			typedModel.Username.Should().Equal(model.Username);
			result.ViewName.Should().Equal("");

			var modelState = result.ViewData.ModelState;
			modelState.ContainsKey("Fake error").Should().Be.True();
		}

		[Test]
		public void GivenReservedUsername_ReturnsRegisterView()
		{
			var model = new Register
			{
				Username = "admin",
				Email = GetRandom.Email(),
				Password = "password1"
			};

			var result = Controller.Register(model) as ViewResult;
			result.Should().Not.Be.Null();

			result.Model.Should().Be.OfType<Register>();
			result.ViewName.Should().Equal("");
		}

		[Test]
		public void GivenDuplicateUsername_ReturnsRegisterView()
		{
			var model = new Register
			{
				Username = User.Username,
				Password = "password1"
			};

			var result = Controller.Register(model) as ViewResult;
			result.Should().Not.Be.Null();

			result.Model.Should().Be.OfType<Register>();
			result.ViewName.Should().Equal("");
		}

		[Test]
		public void GivenDuplicateEmail_ReturnsRegisterView()
		{
			var model = new Register
			{
				Username = GetRandom.String(20),
				Email = User.Email,
				Password = "password1"
			};

			var result = Controller.Register(model) as ViewResult;
			result.Should().Not.Be.Null();

			result.Model.Should().Be.OfType<Register>();
			result.ViewName.Should().Equal("");
		}

		[Test]
		public void GivenValidPostRequest_SendsEmail()
		{
			var model = new Register
			{
				Username = GetRandom.String(20),
				Email = GetRandom.Email(),
				Password = "password1"
			};

			Controller.Register(model);

			MailController.Verify(x => x.Welcome(It.Is<ViewModels.Mail.Welcome>(m => m.To == model.Email && m.Username == model.Username)), Times.Once());
		}
	}
}
