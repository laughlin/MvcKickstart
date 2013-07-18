using MvcContrib.TestHelper;
using KickstartTemplate.Controllers;
using NUnit.Framework;

namespace KickstartTemplate.Tests.Routing
{
	public class ErrorTests : RouteTestBase
	{
		[Test]
		public void DefaultRoute()
		{
			"~/Error".ShouldMapTo<ErrorController>(x => x.Index());
		}
		[Test]
		public void InvalidPageRoute()
		{
			"~/Invalid-Page".ShouldMapTo<ErrorController>(x => x.InvalidPage());
		}
		[Test]
		public void NoPermissionRoute()
		{
			"~/No-Permission".ShouldMapTo<ErrorController>(x => x.NoPermission());
		}
	}
}
