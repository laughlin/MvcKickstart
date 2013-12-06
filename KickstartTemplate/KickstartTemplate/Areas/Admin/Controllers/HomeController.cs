using System.Data;
using System.Web.Mvc;
using KickstartTemplate.Areas.Admin.ViewModels.Home;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Infrastructure.Attributes;
using MvcKickstart.Infrastructure;
using ServiceStack.CacheAccess;

namespace KickstartTemplate.Areas.Admin.Controllers
{
	[RouteArea("admin")]
	public class HomeController : BaseController
    {
		public HomeController(IDbConnection db, ICacheClient cache, IMetricTracker metrics) : base(db, cache, metrics)
		{
		}

		[HttpGet, Route("", Name = "Admin_Home_Index")]
		[Restricted(RequireAdmin = true)]
		public ActionResult Index()
		{
			var model = new Index();
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
