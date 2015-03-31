using KickstartTemplate.Controllers;
using KickstartTemplate.Tests.Utilities;

namespace KickstartTemplate.Tests.Controllers.Home
{
	public abstract class ControllerTestBase : TestBase
	{
		protected HomeController Controller { get; set; }

		public override void Setup()
		{
			base.Setup();

			Controller = new HomeController(Db, Cache, Metrics);
			ControllerUtilities.SetupControllerContext(Controller);
		}
	}
}
