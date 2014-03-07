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
			var attributeRoutingMapperType =  GetInternalMapperType();

			if (attributeRoutingMapperType == null)
			{
				const string MissingTypeMessage = "Internal type System.Web.Mvc.Routing.AttributeRoutingMapper not found. " +
					"You may have updated ASP MVC to a version later than 5.1.1 " +
					" Check online for a new version of MvcRouteTester";
				throw new ApplicationException(MissingTypeMessage);
			}

			var mapMvcAttributeRoutesMethod = GetMapRoutesMethod(attributeRoutingMapperType);

			if (mapMvcAttributeRoutesMethod == null)
			{
				const string MissingMethodMessage = "Internal method System.Web.Mvc.AttributeRoutingMapper.MapAttributeRoutes not found. " +
					"You may have updated ASP MVC to a version later than 5.1.1 " +
					" Check online for a new version of MvcRouteTester";
				throw new ApplicationException(MissingMethodMessage);
			}

			mapMvcAttributeRoutesMethod.Invoke(null, new object[] { routes, controllers });
		}

		private static Type GetInternalMapperType()
		{
			var mvcAssembly = Assembly.Load("System.Web.Mvc, Version=5.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			if (mvcAssembly == null)
			{
				return null;
			}
			return mvcAssembly.GetType("System.Web.Mvc.Routing.AttributeRoutingMapper");
		}

		private static MethodInfo GetMapRoutesMethod(Type attributeRoutingMapperType)
		{
			return attributeRoutingMapperType.GetMethod(
				"MapAttributeRoutes",
				BindingFlags.Public | BindingFlags.Static,
				null,
				new[] { typeof(RouteCollection), typeof(IEnumerable<Type>) },
				null);
		}
	}
}