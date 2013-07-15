using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting.Web.Mvc;

namespace MvcKickstart
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// Note: All routes should be handled by the RouteAttribute on each action
			routes.MapAttributeRoutes(config => config.AddRoutesFromAssemblyOf<RouteConfig>());
		}
	}
}