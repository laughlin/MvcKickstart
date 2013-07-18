using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Moq;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Infrastructure.Attributes;
using KickstartTemplate.Models.Users;
using Should.Fluent;

namespace KickstartTemplate.Tests.Utilities
{
	public static class ControllerUtilities
	{
		/// <summary>
		/// Will mock the user information for the current request
		/// </summary>
		/// <param name="controller">Controller to add user information to</param>
		public static void SetupControllerContext(Controller controller)
		{
			SetupControllerContext(controller, null);
		}

		/// <summary>
		/// Will mock the user information for the current request
		/// </summary>
		/// <param name="controller">Controller to add user information to</param>
		/// <param name="user">Data representing logged in user</param>
		/// <param name="requestFormValues">Data to be returned by Request.Form</param>
		public static void SetupControllerContext(Controller controller, User user, NameValueCollection requestFormValues = null)
		{
			controller.Should().Not.Be.Null();

			if (user == null)
				user = new User { Username = string.Empty };

			var identity = new GenericIdentity(user.Username);
			var principal = new UserPrincipal(user, identity);

			var httpContext = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			var cookies = new HttpCookieCollection();
			request.Setup(x => x.Cookies).Returns(cookies);
			request.Setup(x => x.ApplicationPath).Returns("/");
			request.Setup(x => x.AppRelativeCurrentExecutionFilePath).Returns("~/");
			request.Setup(x => x.PathInfo).Returns(string.Empty);
			request.Setup(x => x.Form).Returns(requestFormValues);
			request.Setup(x => x.Headers).Returns(new NameValueCollection());
			var response = new Mock<HttpResponseBase>();
			response.Setup(x => x.Cookies).Returns(cookies);
			response.Setup(x => x.SetCookie(It.IsAny<HttpCookie>())).Callback<HttpCookie>(x => cookies.Add(x));
			response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns((string virtualPath) => virtualPath);
			var server = new Mock<HttpServerUtilityBase>();

			var tempPath = Path.GetTempPath();
			server.Setup(x => x.MapPath(It.IsAny<string>())).Returns((string input) =>
			{
				var value = input;
				if (input.StartsWith("~"))
				{
					value = value.Substring(1);
				}
				if (value.StartsWith("/"))
				{
					value = value.Substring(1);
				}
				return Path.Combine(tempPath, value);
			});
			response.Setup(x => x.Headers).Returns(new NameValueCollection());
			response.SetupProperty(x => x.StatusCode);
			response.Setup(x => x.AddHeader(It.IsAny<string>(), It.IsAny<string>())).Callback((string s1, string s2) => response.Object.Headers.Add(s1, s2));
			httpContext.Setup(x => x.User).Returns(principal);
			httpContext.Setup(x => x.Request).Returns(request.Object);
			httpContext.Setup(x => x.Response).Returns(response.Object);
			httpContext.Setup(x => x.Server).Returns(server.Object);

			var mockContext = new Mock<ControllerContext>();
			mockContext.Setup(x => x.HttpContext).Returns(httpContext.Object);

			//Create old HttpContext.Current that's used outside of controllers, so we can get the user
			HttpContext.Current = new HttpContext(
				new HttpRequest("", "http://tempuri.org", ""),
				new HttpResponse(new StringWriter())
				) { User = principal };

			controller.ControllerContext = mockContext.Object;
			var mockUrlHelper = new Mock<UrlHelper>(mockContext.Object.RequestContext);
			controller.Url = mockUrlHelper.Object;
		}

		/// <summary>
		/// Asserts that a controller is decorated with the Restricted attribute.
		/// </summary>
		/// <remarks>
		///	Seems like there should be a better way to verify that an action requires authentication...
		/// But this is the best I could find for *UNIT* tests. We are testing the code, not the system....
		/// </remarks>
		/// <typeparam name="T">The type of controller being tested</typeparam>
		/// <param name="controller">Controller of which to assert decoration</param>
		/// <param name="expression">An expression indicating which action method to test.</param>
		/// <param name="requireAdmin">If the action requires an admin</param>
		public static void ActionShouldBeDecoratedWithRestricted<T>(this T controller, 
			Expression<Func<T, ActionResult>> expression,
			bool? requireAdmin = null)
		{
			var type = controller.GetType();
			var member = expression.Body as MethodCallExpression;
			member.Should().Not.Be.Null();

			var actionAuthAttributes = member.Method.GetCustomAttributes(typeof (RestrictedAttribute), true);
			var controllerAuthAttributes = type.GetCustomAttributes(typeof(RestrictedAttribute), true);
			var effectiveAuthAttribute = (actionAuthAttributes.FirstOrDefault() ?? controllerAuthAttributes.FirstOrDefault()) as RestrictedAttribute;

			var actionAnonAttributes = member.Method.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
			var controllerAnonAttributes = type.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
			var effectiveAnonAttribute = (actionAnonAttributes.FirstOrDefault() ?? controllerAnonAttributes.FirstOrDefault()) as AllowAnonymousAttribute;

			effectiveAnonAttribute.Should().Be.Null();
			effectiveAuthAttribute.Should().Not.Be.Null();

			if (requireAdmin != null)
			{
				effectiveAuthAttribute.RequireAdmin.Should().Equal(requireAdmin.Value);
			}
		}
	}
}
