using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcKickstart.Infrastructure;
using StackExchange.Profiling;
using StructureMap;

namespace MvcKickstart
{
	public class MvcApplication : HttpApplication
	{
		public MvcApplication()
		{
			AuthenticateRequest += (sender, e) =>
			{
				var app = (HttpApplication) sender;
				if (Request.IsLocal || (app.User != null && app.User.Identity.IsAuthenticated && app.User.Identity.Name == "admin"))
				{
					MiniProfiler.Start();
				}
			};
			EndRequest += (sender, e) =>
			{
				MiniProfiler.Stop();
				ObjectFactory.ReleaseAndDisposeAllHttpScopedObjects();
			};
		}
		protected void Application_Start()
		{
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new RazorViewEngine());

			AreaRegistration.RegisterAllAreas();

			LoggingConfig.Bootstrap();
			IocConfig.Bootstrap();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			AutomapperConfig.CreateMappings();
			DbConfig.Bootstrap();
		}

		public override string GetVaryByCustomString(HttpContext context, string custom)
		{
			var customs = custom.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
			var cacheKey = string.Empty;
			foreach (var type in customs)
			{
				switch (type)
				{
					case VaryByCustom.User:
						cacheKey += "ByUser_" + (context.User.Identity.IsAuthenticated ? context.User.Identity.Name : string.Empty);
						break;
					case VaryByCustom.UserIsAuthenticated:
						cacheKey += "ByUserIsAuthenticated_" + (context.User.Identity.IsAuthenticated ? "user" : "anon");
						break;
					case VaryByCustom.Ajax:
						var requestBase = new HttpRequestWrapper(context.Request);
						cacheKey += "ByAjax_" + requestBase.IsAjaxRequest();
						break;
				}
			}
			return cacheKey;
		}
	}
}