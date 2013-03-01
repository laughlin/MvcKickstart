using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcKickstart.Infrastructure;
using StructureMap;

namespace MvcKickstart
{
	public class IocConfig
	{
		public static void Bootstrap()
		{
			ObjectFactory.Initialize(x => x.AddRegistry(new IocRegistry()));

			DependencyResolver.SetResolver(new StructureMapDependencyResolver(ObjectFactory.Container));
			// Override signalr's default dependency resolver
			//GlobalHost.DependencyResolver = new StructureMapSignalrDependencyResolver(ObjectFactory.Container);
			//RouteTable.Routes.MapHubs();
		}
	}
}