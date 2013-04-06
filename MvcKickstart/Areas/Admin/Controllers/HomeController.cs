using System.Data;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using MvcKickstart.Areas.Admin.ViewModels.Home;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Attributes;

namespace MvcKickstart.Areas.Admin.Controllers
{
	[RouteArea("admin")]
	public class HomeController : BaseController
    {
		public HomeController(IDbConnection db, IMetricTracker metrics) : base(db, metrics)
		{
		}

		[GET("", RouteName = "Admin_Home_Index")]
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
