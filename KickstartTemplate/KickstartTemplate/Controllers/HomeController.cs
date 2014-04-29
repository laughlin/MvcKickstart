using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Mvc;
using System.Xml.Linq;
using CacheStack.DonutCaching;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Infrastructure.Extensions;
using KickstartTemplate.ViewModels.Home;
using KickstartTemplate.ViewModels.Shared;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Extensions;
using ServiceStack.CacheAccess;
using StackExchange.Profiling;

namespace KickstartTemplate.Controllers
{
	public class HomeController : BaseController
	{
		public HomeController(IDbConnection db, ICacheClient cache, IMetricTracker metrics) : base(db, cache, metrics)
		{
		}

		[HttpGet, Route("", Name = "Home_Index")]
		[DonutOutputCache]
		public ActionResult Index()
		{
			var model = new Index();
			return View(model);
		}

		[HttpGet, Route("sitemap.xml")]
		public FileResult SiteMap()
		{
			var doc = new XDocument();
			XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
			var urlset = new XElement(ns + "urlset");

			urlset.Add(new XElement(ns + "url",
						new XElement(ns + "loc", Url.Absolute(Url.Home().Index())),
						new XElement(ns + "priority", "1.0")));

			// TODO: Add in additional urls as needed

			doc.Add(urlset);

			using (var ms = new MemoryStream())
			{
				doc.Save(ms);

				return File(ms.ToArray(), "text/xml");
			}
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

		[Route("__partial__Home_Menu")]
		[DonutOutputCache(VaryByParam = "nav", VaryByCustom = VaryByCustom.User)]
		public ActionResult Menu(Navigation nav)
		{
			var model = new Menu
				{
					Nav = nav
				};
			return PartialView("_Menu", model);
		}

		[Route("__partial__Home_Analytics")]
		[DonutOutputCache]
		public ActionResult Analytics()
		{
			var model = new Analytics
				{
					Id = ConfigurationManager.AppSettings["Analytics:Id"],
					Domain = ConfigurationManager.AppSettings["Analytics:Domain"]
				};

			if (string.IsNullOrEmpty(model.Id))
				return new EmptyResult();

			if (string.IsNullOrEmpty(model.Domain))
				model.Domain = "auto";

			return PartialView("_Analytics", model);
		}

		[Route("__partial__Home_TagManager")]
		[DonutOutputCache]
		public ActionResult TagManager()
		{
			var model = new TagManager
				{
					Id = ConfigurationManager.AppSettings["Analytics:TagManagerId"],
				};

			if (string.IsNullOrEmpty(model.Id))
				return new EmptyResult();

			return PartialView("_TagManager", model);
		}

		#endregion
	}
}