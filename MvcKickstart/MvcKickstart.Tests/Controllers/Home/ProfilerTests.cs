using System;
using System.Web.Mvc;
using Moq;
using KickstartTemplate.Controllers;
using KickstartTemplate.Models.Users;
using KickstartTemplate.Tests.Utilities;
using NUnit.Framework;
using Should.Fluent;

namespace KickstartTemplate.Tests.Controllers.Home
{
	public class ProfilerTests : ControllerTestBase
	{
		[Test]
		public void GivenAnonymousUser_ReturnsEmptyResult()
		{
			var controller = new HomeController(Db, Metrics, Cache);

			ControllerUtilities.SetupControllerContext(controller);

			var result = controller.Profiler() as EmptyResult;
			result.Should().Not.Be.Null();
		}		
		
		[Test]
		public void GivenInvalidPermissions_ReturnsEmptyResult()
		{
			var controller = new HomeController(Db, Metrics, Cache);

			ControllerUtilities.SetupControllerContext(controller, new User { Username = "testUser" });

			var result = controller.Profiler() as EmptyResult;
			result.Should().Not.Be.Null();
		}

		[Test]
		public void GivenValidPermissions_ReturnsStringResult()
		{
			var controller = new HomeController(Db, Metrics, Cache);

			ControllerUtilities.SetupControllerContext(controller, new User
				                                                       {
					                                                       Username = "testUser",
					                                                       IsAdmin = true
				                                                       });

			// I check for thrown exception, rather than a ViewResult because raven profiler uses HttpContext, which cannot be mocked.
			//Assert.Throws<ArgumentNullException>(() => controller.Profiler());
			var result = controller.Profiler() as ContentResult;
			result.Should().Not.Be.Null();
		}

	}
}
