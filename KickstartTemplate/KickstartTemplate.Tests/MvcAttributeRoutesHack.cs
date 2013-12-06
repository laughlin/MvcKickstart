using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace KickstartTemplate.Tests
{
	class MvcAttributeRoutesHack
	{
		public static void MapAttributeRoutes()
		{
			var controllers = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(asm => asm.GetTypes().Where(t => t != null && t.IsPublic && !t.IsAbstract && typeof (IController).IsAssignableFrom(t)))
				.ToList();

			MapAttributeRoutesInControllers(RouteTable.Routes, controllers);
		}

		public static void MapAttributeRoutesInControllers(RouteCollection routes, IEnumerable<Type> controllers)
		{
			var mapMvcAttributeRoutesMethod = typeof(RouteCollectionAttributeRoutingExtensions)
				.GetMethod("MapMvcAttributeRoutes", 
				BindingFlags.NonPublic | BindingFlags.Static,
				null,
				new[] { typeof(RouteCollection), typeof(IEnumerable<Type>) },
				null);

			if (mapMvcAttributeRoutesMethod == null)
			{
				const string MissingMethodMessage = "Internal method System.Web.Mvc.RouteCollectionAttributeRoutingExtensions.MapMvcAttributeRoutes not found. " +
					"You may have updated ASP MVC to a version later than 5.0. " +
					" Check online fir a new version of MvcRouteTester";
				throw new ApplicationException(MissingMethodMessage);
			}

			mapMvcAttributeRoutesMethod.Invoke(null, new object[] { routes, controllers });
		}
	}
}
