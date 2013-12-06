using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
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
