using System;
using System.Web.Routing;

namespace MvcKickstart.Tests.Routing
{
	public abstract class RouteTestBase : IDisposable
	{
		protected RouteTestBase()
		{
			if (RouteTable.Routes == null || RouteTable.Routes.Count == 0)
				RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		public void Dispose()
		{
			RouteTable.Routes.Clear();
		}
	}
}
