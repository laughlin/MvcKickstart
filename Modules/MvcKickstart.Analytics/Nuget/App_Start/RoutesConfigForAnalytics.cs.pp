using System.Reflection;
using System.Web.Routing;
using AttributeRouting.Web.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.RouteConfigForAnalytics), "PreStart")]

namespace $rootnamespace$ 
{
	public static class RouteConfigForAnalytics
	{
		public static void PreStart() 
		{
			RouteTable.Routes.MapAttributeRoutes(config => 
			{
				config.AddRoutesFromAssembly(Assembly.GetAssembly(typeof(MvcKickstart.Analytics.Controllers.WidgetController)));
			});
		}
	}
}
