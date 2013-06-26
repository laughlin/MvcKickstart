using System.Data;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using CacheStack;
using MvcKickstart.Models.Users;
using MvcKickstart.Services;
using MvcKickstart.ViewModels.Shared;
using ServiceStack.CacheAccess;
using ServiceStack.Logging;
using StructureMap;

namespace MvcKickstart.Infrastructure
{
	public abstract class BaseController : Controller, IWithCacheContext
	{
		protected IDbConnection Db { get; private set; }
		protected IMetricTracker Metrics { get; private set; }
		protected ILog Log { get; private set; }
		protected ICacheClient Cache { get; private set; }
		/// <summary>
		/// Used to set the cache context for donut cached actions
		/// </summary>
		public ICacheContext CacheContext { get; private set; }

		public new UserPrincipal User
		{
			get
			{
				return (UserPrincipal) base.User;
			}
		}

		protected BaseController(IDbConnection db, IMetricTracker metrics, ICacheClient cache)
		{
			Db = db;
			Metrics = metrics;
			Log = LogManager.GetLogger(GetType());
			Cache = cache;
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
			if (filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User.Identity.AuthenticationType == "Forms")
			{
				var userService = ObjectFactory.GetInstance<IUserService>();
				user = userService.GetByUsername(filterContext.HttpContext.User.Identity.Name);
				// Something happened to their account - log them out
				if (user == null || user.IsDeleted)
				{
					// Since this is a rarity, I'm not going to force very controller to inject the userservice in the constructor
					var authService = ObjectFactory.GetInstance<IUserAuthenticationService>();
					authService.Logout();
					filterContext.HttpContext.User = null;
				}
			}
			if (user == null)
			{
				user = new User();
			}

			var identity = filterContext.HttpContext.User != null ? filterContext.HttpContext.User.Identity : new GenericIdentity(user.Username ?? string.Empty);
			filterContext.HttpContext.User = new UserPrincipal(user, identity);

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
		/// <param name="message">Notification message</param>
		protected void NotifySuccess(string message)
		{
			Notify(message, NotificationType.Success);
		}
		/// <summary>
		/// Specify a info notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		protected void NotifyInfo(string message)
		{
			Notify(message, NotificationType.Info);
		}
		/// <summary>
		/// Specify a warning notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		protected void NotifyWarning(string message)
		{
			Notify(message, NotificationType.Warning);
		}
		/// <summary>
		/// Specify an error notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		protected void NotifyError(string message)
		{
			Notify(message, NotificationType.Error);
		}
		/// <summary>
		/// Specify a notification to be shown this request
		/// </summary>
		/// <param name="message">Notification message</param>
		/// <param name="type">Notification type</param>
		protected void Notify(string message, NotificationType type)
		{
			TempData[ViewDataConstants.Notification] = new Notification(message, type);
		}
	}
}