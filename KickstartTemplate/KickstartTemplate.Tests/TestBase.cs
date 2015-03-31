using System.Data;
using Moq;
using KickstartTemplate.Tests.Utilities;
using MvcKickstart.Infrastructure;
using NUnit.Framework;
using ServiceStack.CacheAccess;
using StructureMap;

namespace KickstartTemplate.Tests
{
	public abstract class TestBase
	{
		protected IDbConnection Db { get; set; }
		protected IMetricTracker Metrics { get; set; }
		protected Mock<IMetricTracker> MetricsMock { get; set; }
		protected ICacheClient Cache { get; set; }
		protected Generator Generator { get; set; }
		protected Terminator Terminator { get; set; }

		[TestFixtureSetUp]
		public virtual void SetupFixture()
		{
			Cache = new MemoryCacheClient();
			ObjectFactory.Inject(typeof(ICacheClient), Cache);

			Db = ObjectFactory.GetInstance<IDbConnection>();
			Generator = new Generator(Db);
			Terminator = new Terminator(Db);
		}
		[TestFixtureTearDown]
		public virtual void TearDownFixture()
		{
		}

		[SetUp]
		public virtual void Setup()
		{
			MetricsMock = new Mock<IMetricTracker>();
			Metrics = MetricsMock.Object;
		}

		[TearDown]
		public virtual void TearDown()
		{
		}
	}
}