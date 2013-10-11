using System.Web.Routing;
using AttributeRouting.Web.Mvc;
using KickstartTemplate.Infrastructure;

[assembly: WebActivator.PreApplicationStartMethod(typeof(KickstartTemplate.AttributeRoutingConfig), "Start")]

namespace KickstartTemplate 
{
    public static class AttributeRoutingConfig
	{
		public static void RegisterRoutes(RouteCollection routes) 
		{    
			// See http://github.com/mccalltd/AttributeRouting/wiki for more options.
			// To debug routes locally using the built in ASP.NET development server, go to /routes.axd
            
			routes.MapAttributeRoutes(x =>
				{
					x.AddRoutesFromAssemblyOf<BaseController>();
					x.UseLowercaseRoutes = true;
				});
		}

        public static void Start() 
		{
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
