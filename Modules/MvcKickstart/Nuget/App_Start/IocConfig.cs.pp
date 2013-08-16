using System.Web.Mvc;
using MvcKickstart.Infrastructure;
using StructureMap;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.IocConfig), "PreStart", Order = -100)]

namespace $rootnamespace$ 
{
	public static class IocConfig
	{
		public static void PreStart() 
		{
			// If changes need to be made to IocRegistry, please subclass it and replace the following line
			ObjectFactory.Initialize(x => x.AddRegistry(new IocRegistry(typeof(IocConfig).Assembly)));
			DependencyResolver.SetResolver(new StructureMapDependencyScope(ObjectFactory.Container));
		}
	}
}
