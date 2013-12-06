using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(KickstartTemplate.AttributeRoutingConfig), "Start")]

namespace KickstartTemplate 
{
    public static class AttributeRoutingConfig
	{
		public static void RegisterRoutes(RouteCollection routes) 
		{
			routes.LowercaseUrls = true;
			routes.MapMvcAttributeRoutes();
		}

        public static void Start() 
		{
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
