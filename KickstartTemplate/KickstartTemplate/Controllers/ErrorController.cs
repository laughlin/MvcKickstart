using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CacheStack.DonutCaching;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.ViewModels.Error;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Attributes;
using ServiceStack.CacheAccess;

namespace KickstartTemplate.Controllers
{
	public class ErrorController : BaseController
	{
		public ErrorController(IDbConnection db, ICacheClient cache, IMetricTracker metrics) : base (db, cache, metrics){}

		[GetOrPost("Error", RouteName = "Error_Index")]
		[DonutOutputCache]
		public ActionResult Index()
		{
			return View("Error");
		}

		[GetOrPost("Invalid-Page", RouteName = "Error_InvalidPage")]
		[DonutOutputCache]
		public ActionResult InvalidPage()
		{
			var model = new InvalidPage();

			// Try to grab their original url so we can do suggestions?
			var values = Request.QueryString != null && Request.QueryString.Count > 0 ? Request.QueryString.GetValues(0) : null;
			if (values != null)
			{
				var value = values.FirstOrDefault();
				if (!string.IsNullOrEmpty(value) && value.StartsWith("404;"))
				{
					// We were directed to this page by IIS.
					Metrics.Increment(Metric.Error_404);

					// TODO: Add smarter logic for suggesting pages?
//					var url = value.Split(new[] { "404;" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
//					var action = (string.IsNullOrEmpty(url) ? string.Empty : url.Substring(url.LastIndexOf('/'))).TrimStart('/');
				}
			}

			Response.StatusCode = (int) HttpStatusCode.NotFound;
			return View(model);
		}

		[GetOrPost("No-Permission", RouteName = "Error_NoPermission")]
		[DonutOutputCache]
		public ActionResult NoPermission()
		{
			return View();
		}
	}
}