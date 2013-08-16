using System.Data;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using KickstartTemplate.Models.Users;
using KickstartTemplate.Services;
using MvcKickstart.Infrastructure;
using ServiceStack.CacheAccess;
using StructureMap;

namespace KickstartTemplate.Infrastructure
{
	public abstract class BaseController : MvcKickstart.Infrastructure.ControllerBase
	{
		public new UserPrincipal User
		{
			get
			{
				return (UserPrincipal) base.User;
			}
		}

		protected BaseController()
		{
		}
		protected BaseController(IDbConnection db) : base(db)
		{
		}
		protected BaseController(ICacheClient cache) : base(cache)
		{
		}
		protected BaseController(IDbConnection db, ICacheClient cache) : base(db, cache)
		{
		}
		protected BaseController(IDbConnection db, ICacheClient cache, IMetricTracker metrics) : base(db, cache, metrics)
		{
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
	}
}