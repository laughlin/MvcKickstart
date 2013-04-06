using MvcKickstart.Areas.Admin.Controllers;
using MvcKickstart.Models.Users;
using MvcKickstart.Tests.Utilities;

namespace MvcKickstart.Tests.Controllers.Admin.Home
{
	public abstract class ControllerTestBase : TestBase
	{
		protected User User { get; private set; }
		protected HomeController Controller { get; set; }

		public override void Setup()
		{
			base.Setup();

			User = Generator.SetupUser(x =>
				{
					x.Username = "admin";
					x.IsAdmin = true;
				});

			Controller = new HomeController(Db, Metrics);
			ControllerUtilities.SetupControllerContext(Controller, User);
		}
	}
}
