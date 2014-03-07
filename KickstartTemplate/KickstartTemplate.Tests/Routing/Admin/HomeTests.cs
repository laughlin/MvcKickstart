using KickstartTemplate.Tests.Extensions;
using KickstartTemplate.Areas.Admin.Controllers;
using NUnit.Framework;

namespace KickstartTemplate.Tests.Routing.Admin
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
