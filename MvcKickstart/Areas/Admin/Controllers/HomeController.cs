using System.Data;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CacheStack;
using MvcKickstart.Areas.Admin.ViewModels.Home;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Attributes;
using MvcKickstart.Infrastructure.Extensions;
using MvcKickstart.Models;
using MvcKickstart.Services;
using ServiceStack.CacheAccess;
using Spruce;

namespace MvcKickstart.Areas.Admin.Controllers
{
	[RouteArea("admin")]
	public class HomeController : BaseController
    {
		private ISiteSettingsService _siteSettingsService;
		public HomeController(IDbConnection db, IMetricTracker metrics, ICacheClient cache, ISiteSettingsService siteSettingsService) : base(db, metrics, cache)
		{
			_siteSettingsService = siteSettingsService;
		}

		[GET("", RouteName = "Admin_Home_Index")]
		[Restricted(RequireAdmin = true)]
		public ActionResult Index()
		{
			var model = new Index();
			return View(model);
		}

		[GET("auth", RouteName = "Admin_Home_Auth")]
		[Restricted(RequireAdmin = true)]
		public ActionResult Auth()
		{
			const string scope = "https://www.google.com/analytics/feeds/";
			var next = Url.Absolute(Url.Admin().AuthResponse());
			var url = AuthSubUtil.getRequestUrl(next, scope, false, true);
			return Redirect(url);
		}

		[GET("authResponse", RouteName = "Admin_Home_AuthResponse")]
		[Restricted(RequireAdmin = true)]
		public ActionResult AuthResponse(string token)
		{
			var sessionToken = AuthSubUtil.exchangeForSessionToken(token, null);

			var settings = _siteSettingsService.GetSettings();
			settings.AnalyticsToken = sessionToken;
			Db.Save(settings);
			Cache.Trigger(TriggerFor.Id<SiteSettings>(settings.Id));

			return RedirectToAction("Config");
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
