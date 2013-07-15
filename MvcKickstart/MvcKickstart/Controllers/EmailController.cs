using System.Data;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Extensions;
using MvcKickstart.ViewModels.Mail;
using ServiceStack.CacheAccess;

namespace MvcKickstart.Controllers
{
	public class EmailController : BaseController
	{
		public EmailController(IDbConnection db, IMetricTracker metrics, ICacheClient cache) : base(db, metrics, cache)
		{
		}

		[GET("email", RouteName = "Email_Index")]
		public ActionResult Index()
		{
			return Redirect(Url.Home().Index());
		}

		[GET("email/view", RouteName = "Email_View")]
		public ActionResult TrackView(string id)
		{
			// TODO: Track that the view happened
			return File("~/content/images/blank.gif", "image/gif");
		}

		[GET("email/click", RouteName = "Email_Click")]
		public ActionResult TrackClick(TrackClick model)
		{
			// TODO: Track the click

			return Redirect(model.Url);
		}
	}
}