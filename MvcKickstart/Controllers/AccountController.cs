using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using AutoMapper;
using Dapper;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Attributes;
using MvcKickstart.Infrastructure.Extensions;
using MvcKickstart.Models.Users;
using MvcKickstart.Services;
using MvcKickstart.ViewModels.Account;
using ServiceStack.Text;

namespace MvcKickstart.Controllers
{
    public class AccountController : BaseController
    {
		private readonly IMailController _mailController;
	    private readonly IUserAuthenticationService _authenticationService;

		public AccountController(IDbConnection db, IMetricTracker metrics, IMailController mailController, IUserAuthenticationService authenticationService) : base (db, metrics)
		{
			_mailController = mailController;
			_authenticationService = authenticationService;
		}

		[GET("account", RouteName = "Account_Index")]
		[ConfiguredOutputCache]
		public ActionResult Index()
		{
			return Redirect(Url.Home().Index());
		}

		[GET("account/login", RouteName = "Account_Login")]
		[ConfiguredOutputCache(VaryByParam = "returnUrl", VaryByCustom = VaryByCustom.UserIsAuthenticated)]
		public ActionResult Login(string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			var model = new Login {ReturnUrl = returnUrl};
			return View(model);
		}

		[POST("account/login")]
		public ActionResult Login(Login model)
		{
			if (ModelState.IsValid)
			{
				var user = Db.Query<User>("select * from [{0}] where IsDeleted=0 AND Username=@Username AND Password=@Password".Fmt(Db.GetTableName<User>()), new
					{
						model.Username,
						Password = model.Password.ToSHAHash()
					}
				).SingleOrDefault();
				if (user != null)
				{
					_authenticationService.SetLoginCookie(user, model.RememberMe);
					Metrics.Increment(Metric.Users_SuccessfulLogin);

					if (Url.IsLocalUrl(model.ReturnUrl))
						return Redirect(model.ReturnUrl);
					return RedirectToAction("Index", "Home");
				}
				ModelState.AddModelErrorFor<Login>(x => x.Username, string.Format("The user name or password provided is incorrect. Did you <a href='{0}'>forget your password?</a>", Url.Account().ForgotPassword()));
			}
			Metrics.Increment(Metric.Users_FailedLogin);

			// If we got this far, something failed, redisplay form
			model.Password = null; //clear the password so they have to re-enter it
			return View(model);
		}

		[Restricted]
		[GET("account/logout", RouteName = "Account_Logout")]
		public ActionResult Logout(string returnUrl)
		{
			Metrics.Increment(Metric.Users_Logout);
			_authenticationService.Logout();
			if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction("Index", "Home");
		}

		#region Register

		[GET("account/register", RouteName = "Account_Register")]
		[ConfiguredOutputCache(VaryByParam = "returnUrl", VaryByCustom = VaryByCustom.UserIsAuthenticated)]
		public ActionResult Register(string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			return View(new Register { ReturnUrl = returnUrl });
		}

		[POST("account/register")]
		public ActionResult Register(Register model)
		{
			if (ModelState.IsValid)
			{
				if (_authenticationService.ReservedUsernames.Any(x => model.Username.Equals(x, StringComparison.OrdinalIgnoreCase)))
				{
					ModelState.AddModelErrorFor<Register>(x => x.Username, "Username is unavailable");
				}
				else
				{
					var user = Db.Query<User>("select * from [{0}] where IsDeleted=0 AND (Username=@Username OR Email=@Email)".Fmt(Db.GetTableName<User>()), new
						{
							model.Username,
							model.Email
						}
					).SingleOrDefault();

					if (user != null)
					{
						if (user.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase))
						{
							ModelState.AddModelErrorFor<Register>(x => x.Username, "Username is already in use");
						}
						if (user.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
						{
							ModelState.AddModelErrorFor<Register>(x => x.Email, "A user with that email exists");
						}
					}					
				}
			}

			if (ModelState.IsValid)
			{
				var newUser = Mapper.Map<User>(model);
				newUser.Password = model.Password.ToSHAHash();

				Db.Save(newUser);
				Metrics.Increment(Metric.Users_Register);

				_mailController.Welcome(new ViewModels.Mail.Welcome
					{
						Username = newUser.Username,
						To = newUser.Email
					}).Deliver();

				return View("RegisterConfirmation", model);			}

			// If we got this far, something failed, redisplay form
			model.Password = null; //clear the password so they have to re-enter it
			return View(model);
		}

		[POST("account/validate-username/{username}", RouteName = "Account_ValidateUsername")]
		[ConfiguredOutputCache(VaryByParam = "username")]
		public JsonResult ValidateUsername(string username)
		{
			if (_authenticationService.ReservedUsernames.Any(x => username.Equals(x, StringComparison.OrdinalIgnoreCase)))
				return Json(false);

			var user = Db.Query<User>("select * from [{0}] where IsDeleted=0 AND Username=@Username".Fmt(Db.GetTableName<User>()), new
				{
					username
				}
			).SingleOrDefault();

			return Json(user == null);
		}
		#endregion

		#region Forgot Password
		[GET("account/forgot-password", RouteName = "Account_ForgotPassword")]
		[ConfiguredOutputCache]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		[POST("account/forgot-password")]
		public ActionResult ForgotPassword(ForgotPassword model)
		{
			if (ModelState.IsValid)
			{
				//get user by email address
				var user = Db.Query<User>("select * from [{0}] where IsDeleted=0 AND Email=@Email".Fmt(Db.GetTableName<User>()), new
					{
						model.Email
					}
				).SingleOrDefault();
				
				//if no matching user, error
				if (user == null)
				{
					ModelState.AddModelErrorFor<ForgotPassword>(x => x.Email, "A user could not be found with that email address");
					return View(model);
				}

				// Create token and send email
				var token = new PasswordRetrieval(user, Guid.NewGuid());
				Db.Save(token);
				Metrics.Increment(Metric.Users_SendPasswordResetEmail);

				_mailController.ForgotPassword(new ViewModels.Mail.ForgotPassword
					{
						To = user.Email,
						Token = token.Token
					}).Deliver();
				
				return View("ForgotPasswordConfirmation");

			}
			return View(model);
		}

		[GET("account/reset-password/{token}", RouteName = "Account_ResetPassword")]
		public ActionResult ResetPassword(string token)
		{
			if (User.Identity.IsAuthenticated)
			{
				NotifyInfo("You are already logged in. Log out and try again.");
				return Redirect(Url.Home().Index());
			}
			Guid guidToken;
			if (!Guid.TryParse(token, out guidToken))
			{
				NotifyWarning("Sorry! We couldn't verify that this user requested a password reset. Please try resetting again.");
				return Redirect(Url.Account().ForgotPassword());
			}

			var model = new ResetPassword
				{
					Token = guidToken, 
					Data = Db.Query<PasswordRetrieval>("select * from [{0}] where Token=@Token".Fmt(Db.GetTableName<PasswordRetrieval>()), new
						{
							Token = guidToken
						}).SingleOrDefault()
				};

			if (model.Data == null)
			{
				NotifyWarning("Sorry! We couldn't verify that this user requested a password reset. Please try resetting again.");
				return Redirect(Url.Account().ForgotPassword());
			}

			return View(model);
		}

		[POST("account/reset-password/{token}")]
		public ActionResult ResetPassword(ResetPassword model)
		{
			if (User.Identity.IsAuthenticated)
			{
				NotifyInfo("You are already logged in. Log out and try again.");
				return Redirect(Url.Home().Index());
			}
			if (ModelState.IsValid)
			{
				model.Data = Db.Query<PasswordRetrieval>("select * from [{0}] where Token=@Token".Fmt(Db.GetTableName<PasswordRetrieval>()), new
						{
							model.Token
						}).SingleOrDefault();

				if (model.Data == null)
				{
					NotifyWarning("Sorry! We couldn't verify that this user requested a password reset. Please try resetting again.");
					return Redirect(Url.Account().ForgotPassword());
				}

				var user = Db.Query<User>("delete from [{0}] where Id=@resetId;update [{1}] set Password=@Password, ModifiedOn=GetUtcDate() where Id=@UserId;select * from [{1}] where Id=@UserId"
					.Fmt(
						Db.GetTableName<PasswordRetrieval>(),
						Db.GetTableName<User>()
					), new
						{
							ResetId = model.Data.Id,
							Password = model.Password.ToSHAHash(),
							model.Data.UserId
						}).SingleOrDefault();
				
				_authenticationService.SetLoginCookie(user, true);

				Metrics.Increment(Metric.Users_ResetPassword);
				//show confirmation
				return View("ResetPasswordConfirmation");
			}
			return View(model);
		}
		#endregion
	}
}
