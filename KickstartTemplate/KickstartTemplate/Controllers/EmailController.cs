using System.Web.Mvc;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Infrastructure.Extensions;
using KickstartTemplate.ViewModels.Mail;

namespace KickstartTemplate.Controllers
{
	public class EmailController : BaseController
	{
		[HttpGet, Route("email", Name = "Email_Index")]
		public ActionResult Index()
		{
			return Redirect(Url.Home().Index());
		}

		[HttpGet, Route("email/view", Name = "Email_View")]
		public ActionResult TrackView(string id)
		{
			// TODO: Track that the view happened
			return File("~/content/images/blank.gif", "image/gif");
		}

		[HttpGet, Route("email/click", Name = "Email_Click")]
		public ActionResult TrackClick(TrackClick model)
		{
			// TODO: Track the click

			return Redirect(model.Url);
		}
	}
}