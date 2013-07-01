using MvcKickstart.Controllers;
using MvcKickstart.Tests.Utilities;
using NUnit.Framework;

namespace MvcKickstart.Tests.Controllers.Home
{
	public abstract class ControllerTestBase : TestBase
	{
		protected HomeController Controller { get; set; }

		public override void Setup()
		{
			base.Setup();

			Controller = new HomeController(Db, Metrics, Cache);
			ControllerUtilities.SetupControllerContext(Controller);
		}
	}
}
