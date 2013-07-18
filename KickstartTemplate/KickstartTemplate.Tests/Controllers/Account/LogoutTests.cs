using System.Web.Mvc;
using Moq;
using KickstartTemplate.Tests.Utilities;
using NUnit.Framework;
using Should.Fluent;

namespace KickstartTemplate.Tests.Controllers.Account
{
	public class LogoutTests : ControllerTestBase
	{
		[Test]
		public void ActionRequiresAuthorization()
		{
			Controller.ActionShouldBeDecoratedWithRestricted(x => x.Logout(""));
		}

		[Test]
		public void GivenUnauthenticatedRequest_ReturnsHomePage()
		{
			var result = Controller.Logout("") as RedirectToRouteResult;
			result.Should().Not.Be.Null();
		}
		[Test]
		public void GivenAuthenticatedRequest_ReturnsHomePage()
		{
			ControllerUtilities.SetupControllerContext(Controller, User);

			var result = Controller.Logout("") as RedirectToRouteResult;
			result.Should().Not.Be.Null();

			AuthenticationService.Verify(x => x.Logout(), Times.Once());
		}
		[Test]
		public void GivenAuthenticatedRequest_ReturnsRedirectToReturnUrl()
		{
			const string returnUrl = "/home/index";

			ControllerUtilities.SetupControllerContext(Controller, User);

			var result = Controller.Logout(returnUrl) as RedirectResult;
			result.Should().Not.Be.Null();
			result.Url.Should().Equal(returnUrl);

			AuthenticationService.Verify(x => x.Logout(), Times.Once());
		}
	}
}
