using System.Reflection;
using System.Web.Routing;
using AttributeRouting.Web.Mvc;
using MvcKickstart.Analytics.Controllers

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.RouteConfigForAnalytics), "PreStart")]

namespace $rootnamespace$ 
{
	public static class RouteConfigForAnalytics
	{
		public static void PreStart() 
		{
			RouteTable.Routes.MapAttributeRoutes(config => 
			{
				config.AddRoutesFromAssembly(Assembly.GetAssembly(typeof(AnalyticsController)));
			});
		}
	}
}
