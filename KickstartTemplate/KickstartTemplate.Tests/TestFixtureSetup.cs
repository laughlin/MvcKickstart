using System.Data;
using System.Web.Routing;
using KickstartTemplate.Infrastructure.Data;
using MvcKickstart.Infrastructure;
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
				AttributeRoutingConfig.RegisterRoutes(RouteTable.Routes);

			IocConfig.PreStart();

			DbConfig.Setup();

			var db = ObjectFactory.GetInstance<IDbConnection>();

			new SchemaBuilder(db).GenerateSchema(true);

			AutomapperConfig.CreateMappings();
		}
	}
}
