using MvcContrib.TestHelper;
using MvcKickstart.Controllers;
using NUnit.Framework;

namespace MvcKickstart.Tests.Routing
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
