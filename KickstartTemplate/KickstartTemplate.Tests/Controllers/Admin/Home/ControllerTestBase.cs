using Moq;
using KickstartTemplate.Areas.Admin.Controllers;
using KickstartTemplate.Models.Users;
using KickstartTemplate.Services;
using KickstartTemplate.Tests.Utilities;

namespace KickstartTemplate.Tests.Controllers.Admin.Home
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
