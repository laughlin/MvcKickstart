using System.Data;
using System.Web.Routing;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Data.Schema;
using NUnit.Framework;
using StructureMap;

namespace MvcKickstart.Tests
{
	[SetUpFixture]
	public class SqlSetup
	{
		[SetUp]
		public void Setup()
		{
			if (RouteTable.Routes == null || RouteTable.Routes.Count == 0)
				RouteConfig.RegisterRoutes(RouteTable.Routes);

			ObjectFactory.Initialize(x => x.AddRegistry(new IocRegistry()));

			var db = ObjectFactory.GetInstance<IDbConnection>();
			new SchemaBuilder(db).GenerateSchema(true);

			AutomapperConfig.CreateMappings();
		}
	}
}
