using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Dapper;
using MvcKickstart.Infrastructure.Extensions;
using MvcKickstart.Models.Users;
using MvcKickstart.ViewModels.Shared;
using ServiceStack.Logging;
using ServiceStack.Text;

namespace MvcKickstart.Infrastructure
{
	public abstract class BaseController : Controller
	{
		protected IDbConnection Db { get; private set; }
		protected IMetricTracker Metrics { get; private set; }
		protected ILog Log { get; private set; }

		public new UserPrincipal User
		{
			get
			{
				return (UserPrincipal) base.User;
			}
		}

		protected BaseController(IDbConnection db, IMetricTracker metrics)
		{
			Db = db;
			Metrics = metrics;
			Log = LogManager.GetLogger(GetType());
		}

		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			// No need to create a new principal object if it already exists (child actions)
			if (filterContext.HttpContext.User is UserPrincipal)
			{
				base.OnAuthorization(filterContext);
				return;
			}

			User user = null;
			if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User.Identity.AuthenticationType == "Forms")
			{
				user = Db.Query<User>("select * from [{0}] where Username=@username".Fmt(Db.GetTableName<User>()), new { Username = filterContext.HttpContext.User.Identity.Name }).SingleOrDefault();
			}
			if (user == null)
			{
				user = new User();
			}

			filterContext.HttpContext.User = new UserPrincipal(user, filterContext.HttpContext.User.Identity);
			Thread.CurrentPrincipal = filterContext.HttpContext.User;
			base.OnAuthorization(filterContext);
		}

		protected override void Execute(System.Web.Routing.RequestContext requestContext)
		{
			base.Execute(requestContext);
			// If this is an ajax request, clear the tempdata notification.
			if (requestContext.HttpContext.Request.IsAjaxRequest())
			{
				TempData[ViewDataConstants.Notification] = null;
			}
		}

		protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new ServiceStackJsonResult
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding
			};
		}

		/// <summary>
		/// Returns the specified error object as json.  Sets the response status code to the ErrorCode value
		/// </summary>
		/// <param name="error">Error to return</param>
		/// <returns></returns>
		protected JsonResult JsonError(Error error)
		{
			return JsonError(error, error.ErrorCode ?? (int) HttpStatusCode.InternalServerError);
		}
		/// <summary>
		/// Returns the specified error object as json.
		/// </summary>
		/// <param name="error">Error to return</param>
		/// <param name="responseCode">StatusCode to return with the response</param>
		/// <returns></returns>
		protected JsonResult JsonError(Error error, int responseCode)
		{
			Response.StatusCode = responseCode;
			return Json(error);
		}

		/// <summary>
		/// Specify a success notification to be shown this request
		/// </summary>
		/// <param name="message"></param>
		protected void NotifySuccess(string message)
		{
			Notify(message, NotificationType.Success);
		}
		/// <summary>
		/// Specify a info notification to be shown this request
		/// </summary>
		/// <param name="message"></param>
		protected void NotifyInfo(string message)
		{
			Notify(message, NotificationType.Info);
		}
		/// <summary>
		/// Specify a warning notification to be shown this request
		/// </summary>
		/// <param name="message"></param>
		protected void NotifyWarning(string message)
		{
			Notify(message, NotificationType.Warning);
		}
		/// <summary>
		/// Specify an error notification to be shown this request
		/// </summary>
		/// <param name="message"></param>
		protected void NotifyError(string message)
		{
			Notify(message, NotificationType.Error);
		}
		/// <summary>
		/// Specify a notification to be shown this request
		/// </summary>
		/// <param name="message"></param>
		protected void Notify(string message, NotificationType type)
		{
			TempData[ViewDataConstants.Notification] = new Notification(message, type);
		}
	}
}