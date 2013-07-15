using Moq;
using MvcKickstart.Areas.Admin.Controllers;
using MvcKickstart.Models.Users;
using MvcKickstart.Services;
using MvcKickstart.Tests.Utilities;

namespace MvcKickstart.Tests.Controllers.Admin.Home
{
	public abstract class ControllerTestBase : TestBase
	{
		protected User User { get; private set; }
		protected HomeController Controller { get; set; }
		protected Mock<SiteSettingsService> SiteSettingsServiceMock { get; set; }

		public override void Setup()
		{
			base.Setup();

			User = Generator.SetupUser(x =>
				{
					x.Username = "admin";
					x.IsAdmin = true;
				});
			SiteSettingsServiceMock = new Mock<SiteSettingsService>(Db, Cache);

			Controller = new HomeController(Db, Metrics, Cache, SiteSettingsServiceMock.Object);
			ControllerUtilities.SetupControllerContext(Controller, User);
		}
	}
}
