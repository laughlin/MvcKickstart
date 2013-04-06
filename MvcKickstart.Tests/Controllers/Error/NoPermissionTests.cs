using System.Web.Mvc;
using MvcKickstart.Controllers;
using MvcKickstart.Tests.Utilities;
using NUnit.Framework;
using Should.Fluent;

namespace MvcKickstart.Tests.Controllers.Error
{
	public class NoPermissionTests : TestBase
	{
		[Test]
		public void GivenRequest_ReturnsNoPermissionPageView()
		{
			var controller = new ErrorController(Db, Metrics);
			ControllerUtilities.SetupControllerContext(controller);

			var result = controller.NoPermission() as ViewResult;
			result.Should().Not.Be.Null();
			result.ViewName.Should().Equal("");
		}
	}
}
