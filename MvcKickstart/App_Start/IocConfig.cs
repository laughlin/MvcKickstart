using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcKickstart.Infrastructure;
using StructureMap;

namespace MvcKickstart
{
	public class IocConfig
	{
		public static void Initialize()
		{
			ObjectFactory.Initialize(x => x.AddRegistry(new IocRegistry()));

			DependencyResolver.SetResolver(new StructureMapDependencyScope(ObjectFactory.Container));
			// Override signalr's default dependency resolver
//			GlobalHost.DependencyResolver = ObjectFactory.GetInstance<IDependencyResolver>();
		}
	}
}