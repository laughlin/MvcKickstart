using System;
using KickstartTemplate.Tests.Extensions;
using KickstartTemplate.Controllers;
using NUnit.Framework;

namespace KickstartTemplate.Tests.Routing
{
	public class AccountTests : RouteTestBase
	{
		[Test]
		public void Index()
		{
			"~/account".ShouldMapTo<AccountController>(x => x.Index());
		}

		[Test]
		public void LoginRoute()
		{
			"~/account/login".ShouldMapTo<AccountController>(x => x.Login((string) null));
		}
		[Test]
		public void LogoutRoute()
		{
			"~/account/logout".ShouldMapTo<AccountController>(x => x.Logout(null));
		}
		[Test]
		public void RegisterRoute()
		{
			"~/account/register".ShouldMapTo<AccountController>(x => x.Register((string) null));
		}
		[Test]
		public void ForgotPassword()
		{
			"~/account/forgot-password".ShouldMapTo<AccountController>(x => x.ForgotPassword());
		}
		[Test]
		public void ResetPassword()
		{
			var id = Guid.NewGuid().ToString("N");
			("~/account/reset-password/" + id).ShouldMapTo<AccountController>(x => x.ResetPassword(id));
		}

	}
}
