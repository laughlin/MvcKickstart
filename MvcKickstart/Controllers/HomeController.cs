using System.Data;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Attributes;
using MvcKickstart.ViewModels.Home;
using StackExchange.Profiling;

namespace MvcKickstart.Controllers
{
	public class HomeController : BaseController
	{
		public HomeController(IDbConnection db, IMetricTracker metrics) : base(db, metrics)
		{
		}

		[GET("", RouteName = "Home_Index")]
		[ConfiguredOutputCache]
		public ActionResult Index()
		{
			var model = new Index();
			return View(model);
		}

		#region Partials

		[Route("__partial__Home_Profiler")]
		public ActionResult Profiler()
		{
			if (User.IsAdmin)
			{
				return Content(MiniProfiler.RenderIncludes().ToHtmlString());
			}
			return new EmptyResult();
		}

		[Route("__partial__Home_Notification")]
		public ActionResult Notification()
		{
			return PartialView("_Notification");
		}

		[Route("__partial__Home_UsernameOrLogin")]
		[ConfiguredOutputCache(VaryByCustom = VaryByCustom.User)]
		public ActionResult UsernameOrLogin()
		{
			// This route could probably also go in a UsersController if it exists
			return PartialView("_UsernameOrLogin");
		}


		#endregion
	}
}