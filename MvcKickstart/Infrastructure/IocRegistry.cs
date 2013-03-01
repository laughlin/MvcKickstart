using System.Configuration;
using System.Data;
using Raven.Client;
using Raven.Client.Document;
using ServiceStack.CacheAccess;
using StructureMap.Configuration.DSL;

namespace MvcKickstart.Infrastructure
{
	public class IocRegistry : Registry
	{
		public IocRegistry()
		{
			Scan(scan =>
					{
						scan.TheCallingAssembly();
						scan.WithDefaultConventions();
					});

			For<IDocumentStore>()
				.Singleton()
				.Use(x =>
						{
							var store = new DocumentStore { ConnectionStringName = "Raven" };
							store.RegisterListener(new DocumentStoreListener());
							store.Initialize();

							MvcMiniProfiler.RavenDb.Profiler.AttachTo(store);

							return store;
						})
				.Named("RavenDB Document Store");

			For<IDocumentSession>()
				.HttpContextScoped()
				.Use(x =>
					{
						var store = x.GetInstance<IDocumentStore>();
						return store.OpenSession();
					})
				.Named("RavenDb Session");

			For<IMetricTracker>()
				.Singleton()
				.Use(x =>
				     	{
				     		int port;
				     		int.TryParse(ConfigurationManager.AppSettings["Metrics:Port"], out port);
				     		return new MetricTracker(ConfigurationManager.AppSettings["Metrics:Host"], port);
				     	})
				.Named("Metric Tracker");

			For<ICacheClient>()
				.Singleton()
				.Use(x => new MemoryCacheClient());
		}
	}
}
