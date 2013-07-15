//using System;
//using System.Collections.Generic;
//using System.Linq;
//using SignalR;
//using StructureMap;

//namespace MvcKickstart.Infrastructure
//{
//	/// <summary>
//	/// SignalR dependency resolver for structuremap
//	/// </summary>
//	public class StructureMapSignalrDependencyResolver : DefaultDependencyResolver
//	{
//		private readonly IContainer _container;

//		public StructureMapSignalrDependencyResolver(IContainer container)
//		{
//			if (container == null)
//			{
//				throw new ArgumentNullException("container");
//			}

//			_container = container;
//		}

//		public override object GetService(Type serviceType)
//		{
//			return !serviceType.IsAbstract && !serviceType.IsInterface && serviceType.IsClass
//							   ? _container.GetInstance(serviceType)
//							   : (_container.TryGetInstance(serviceType) ?? base.GetService(serviceType));
//		}

//		public override IEnumerable<object> GetServices(Type serviceType)
//		{
//			return _container.GetAllInstances(serviceType).Cast<object>().Concat(base.GetServices(serviceType));
//		}
//	}
//}