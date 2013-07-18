using MvcContrib.TestHelper;
using KickstartTemplate.Controllers;
using NUnit.Framework;

namespace KickstartTemplate.Tests.Routing
{
	public class HomeTests : RouteTestBase
	{
		[Test]
		public void DefaultRoute()
		{
			"~/".ShouldMapTo<HomeController>(x => x.Index());
		}
	}
}
