using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ServiceStack.CacheAccess;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace MvcKickstart.Infrastructure
{
	public class IocRegistry : Registry
	{
		public IocRegistry(params Assembly[] additionalAssembliesToScan)
		{
			Scan(scan =>
					{
						scan.TheCallingAssembly();
						if (additionalAssembliesToScan != null)
						{
							foreach (var assembly in additionalAssembliesToScan)
							{
								scan.Assembly(assembly);
							}
						}
						// Make sure all plugins/modules are wired up with default conventions
						scan.AssembliesFromApplicationBaseDirectory(x => x.FullName.Contains("MvcKickstart"));
						scan.AssemblyContainingType<IocRegistry>();
						scan.WithDefaultConventions();
					});

			For<SqlConnection>().Use(() =>
			{
				var connStr = "Default";
				if (ConfigurationManager.ConnectionStrings[Environment.MachineName] != null)
					connStr = Environment.MachineName;
				return new SqlConnection(ConfigurationManager.ConnectionStrings[connStr].ConnectionString);
			});

			For<IDbConnection>()
				.HybridHttpOrThreadLocalScoped()
				.Use(() =>
					{
						var connection = ObjectFactory.GetInstance<SqlConnection>();
						connection.Open();
						return new ProfiledDbConnection(connection, MiniProfiler.Current);						
					})
				.Named("Database Connection");

			For<IMetricTracker>()
				.Singleton()
				.Use(x =>
				     	{
				     		int port;
				     		int.TryParse(ConfigurationManager.AppSettings["Metrics:Port"], out port);
				     		return new MetricTracker(ConfigurationManager.AppSettings["Metrics:Host"], port, ConfigurationManager.AppSettings["Metrics:Prefix"]);
				     	})
				.Named("Metric Tracker");

			For<ICacheClient>()
				.Singleton()
				.Use(x => new MemoryCacheClient());
		}
	}
}
