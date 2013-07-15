using MvcContrib.TestHelper;
using MvcKickstart.Areas.Admin.Controllers;
using NUnit.Framework;

namespace MvcKickstart.Tests.Routing.Admin
{
	public class HomeTests : RouteTestBase
	{
		[Test]
		public void DefaultRoute()
		{
			"~/admin".ShouldMapTo<HomeController>(x => x.Index());
		}
	}
}
