using System.Data;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CacheStack;
using Google.GData.Client;
using KickstartTemplate.Areas.Admin.ViewModels.Home;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Infrastructure.Attributes;
using KickstartTemplate.Infrastructure.Extensions;
using KickstartTemplate.Models;
using KickstartTemplate.Services;
using ServiceStack.CacheAccess;
using Spruce;

namespace KickstartTemplate.Areas.Admin.Controllers
{
	[RouteArea("admin")]
	public class HomeController : BaseController
    {
		private readonly ISiteSettingsService _siteSettingsService;
		public HomeController(IDbConnection db, IMetricTracker metrics, ICacheClient cache, ISiteSettingsService siteSettingsService) : base(db, metrics, cache)
		{
			_siteSettingsService = siteSettingsService;
		}

		[GET("", RouteName = "Admin_Home_Index")]
		[Restricted(RequireAdmin = true)]
		public ActionResult Index()
		{
			var settings = _siteSettingsService.GetSettings();
			var model = new Index
				{
					HasAnalyticsConfigured = !string.IsNullOrEmpty(settings.AnalyticsToken)
				};
			return View(model);
		}

		#region Partials

		[Route("__partial__Menu")]
		public ActionResult Menu()
		{
			if (User.IsAdmin)
			{
				return PartialView("_Menu");
			}
			return new EmptyResult();
		}

		#endregion
	}
}
