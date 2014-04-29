using System;
using System.Web.Mvc;
using KickstartTemplate.ViewModels.Mail;

namespace KickstartTemplate.Infrastructure.Extensions
{
	public static class UrlHelperExtensions
	{
		public static UrlHelperAdminUrls Admin(this UrlHelper helper)
		{
			return new UrlHelperAdminUrls(helper);
		}

		#region Home

		public static HomeUrls Home(this UrlHelper helper)
		{
			return new HomeUrls(helper);
		}

		public class HomeUrls
		{
			public UrlHelper Url { get; set; }

			public HomeUrls(UrlHelper url)
			{
				Url = url;
			}

			public string Index()
			{
				return Url.RouteUrl("Home_Index");
			}
		}

		#endregion

		#region Account

		public static AccountUrls Account(this UrlHelper helper)
		{
			return new AccountUrls(helper);
		}

		public class AccountUrls
		{
			public UrlHelper Url { get; set; }

			public AccountUrls(UrlHelper url)
			{
				Url = url;
			}

			public string Index()
			{
				return Url.RouteUrl("Account_Index");
			}
			public string Login()
			{
				return Url.RouteUrl("Account_Login");
			}
			public string Login(string returnUrl)
			{
				return Url.RouteUrl("Account_Login", new { returnUrl });
			}
			public string Logout()
			{
				return Url.RouteUrl("Account_Logout");
			}

			public string Register()
			{
				return Url.RouteUrl("Account_Register");
			}
			public string ForgotPassword()
			{
				return Url.RouteUrl("Account_ForgotPassword");
			}
			public string ResetPassword(Guid token)
			{
				return Url.RouteUrl("Account_ResetPassword", new { token });
			}
			public string ValidateUsername()
			{
				return Url.RouteUrl("Account_ValidateUsername");
			}
		}

		#endregion
	}
}