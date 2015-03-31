using System.Data;
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
			IocConfig.PreStart();

			DbConfig.Setup();

			var db = ObjectFactory.GetInstance<IDbConnection>();

			new SchemaBuilder(db).GenerateSchema(true);

			AutomapperConfig.CreateMappings();

			// Map routes... This is an annoying hack because the MVC team figured people don't care about testing attribute routes!
			MvcAttributeRoutesHack.MapAttributeRoutes();
		}
	}
}
