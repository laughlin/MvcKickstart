using System.Data;
using System.Web.Routing;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Infrastructure.Data;
using NUnit.Framework;
using StructureMap;

namespace KickstartTemplate.Tests
{
	[SetUpFixture]
	public class TestFixtureSetup
	{
		[SetUp]
		public void Setup()
		{
			if (RouteTable.Routes == null || RouteTable.Routes.Count == 0)
				RouteConfig.RegisterRoutes(RouteTable.Routes);

			ObjectFactory.Initialize(x => x.AddRegistry(new IocRegistry()));

			DbConfig.Setup();

			var db = ObjectFactory.GetInstance<IDbConnection>();

			new SchemaBuilder(db).GenerateSchema(true);

			AutomapperConfig.CreateMappings();
		}
	}
}
