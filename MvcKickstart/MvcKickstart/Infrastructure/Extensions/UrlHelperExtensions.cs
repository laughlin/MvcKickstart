using System;
using System.Configuration;
using System.Web.Mvc;
using MvcKickstart.ViewModels.Mail;
using ServiceStack.CacheAccess;
using ServiceStack.Text;
using StructureMap;

namespace MvcKickstart.Infrastructure.Extensions
{
	public static class UrlHelperExtensions
	{
		private static ICacheClient Cache { get; set; }

		static UrlHelperExtensions()
		{
			Cache = ObjectFactory.GetInstance<ICacheClient>();
		}

		#region Static Assets

		private static string GetFileHash(string filename)
		{
			var key = "__FileHash__" + filename;
			var hash = Cache.Get<string>(key);
			if (hash == null)
			{
				hash = filename.ToMD5Hash();
				Cache.Add(key, hash, new TimeSpan(30, 0, 0, 0));
			}
			return hash;
		}

		private static string GetHashedContentFile(this UrlHelper helper, string filename)
		{
			var hash = GetFileHash(filename);
			return helper.Content(filename) + "?v={0}".FormatWith(hash);
		}

		public static string Image(this UrlHelper helper, string file)
		{
			return helper.GetHashedContentFile("~/Content/images/{0}".FormatWith(file));
		}

		/// <summary>
		/// This extension method will help generating Absolute Urls in the mailer or other views
		/// </summary>
		/// <param name="urlHelper">The object that gets the extended behavior</param>
		/// <param name="relativeOrAbsoluteUrl">A relative or absolute URL to convert to Absolute</param>
		/// <returns>An absolute Url. e.g. http://domain:port/controller/action from /controller/action</returns>
		/// <remarks>Shamelessly stolen from MvcMailer: https://github.com/smsohan/MvcMailer/blob/master/Mvc.Mailer/ExtensionMethods/UrlHelperExtensions.cs</remarks>
		public static string Absolute(this UrlHelper urlHelper, string relativeOrAbsoluteUrl)
		{
			var uri = new Uri(relativeOrAbsoluteUrl, UriKind.RelativeOrAbsolute);
			if (uri.IsAbsoluteUri)
				return relativeOrAbsoluteUrl;

			Uri combinedUri;
			if (Uri.TryCreate(BaseUrl(urlHelper), relativeOrAbsoluteUrl, out combinedUri))
				return combinedUri.AbsoluteUri;

			throw new Exception(string.Format("Could not create absolute url for {0} using baseUri {0}", relativeOrAbsoluteUrl,
			                                  BaseUrl(urlHelper)));
		}

		private static Uri BaseUrl(UrlHelper urlHelper)
		{
			var baseUrl = ConfigurationManager.AppSettings.Get("BaseUrl");

			//No configuration given, so use the one from the context
			if (string.IsNullOrWhiteSpace(baseUrl))
			{
				if (urlHelper.RequestContext.HttpContext.Request.Url != null)
					baseUrl = urlHelper.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
			}

			return new Uri(baseUrl);
		}

		#endregion

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

		#region Email

		public static EmailUrls Email(this UrlHelper helper)
		{
			return new EmailUrls(helper);
		}

		public class EmailUrls
		{
			protected UrlHelper Url { get; set; }

			public EmailUrls(UrlHelper url)
			{
				Url = url;
			}

			public string TrackView(string id)
			{
				return Url.RouteUrl("Email_View", new
					{
						id
					});
			}
			public string TrackClick(Guid id, string url)
			{
				var data = new TrackClick
					{
						Id = id,
						Url = url
					};
				return TrackClick(data);
			}
			public string TrackClick(TrackClick data)
			{
				return Url.RouteUrl("Email_Click", new
					{
						data.Url,
						data.Id
					});
			}
			public string Asset(string filename)
			{
				return Url.Content("~/Content/Email/" + filename);
			}
		}

		#endregion
	}
}