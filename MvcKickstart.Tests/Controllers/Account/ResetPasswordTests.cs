using System;
using System.Linq;
using System.Web.Mvc;
using Dapper;
using FizzWare.NBuilder.Generators;
using Moq;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Extensions;
using MvcKickstart.Models.Users;
using MvcKickstart.Tests.Utilities;
using NUnit.Framework;
using ServiceStack.Text;
using Should.Fluent;
using MvcKickstart.ViewModels.Account;
using Spruce;

namespace MvcKickstart.Tests.Controllers.Account
{
	public class ResetPasswordTests : ControllerTestBase
	{
		[Test]
		public void GivenAuthenticatedGetRequest_Redirects()
		{
			ControllerUtilities.SetupControllerContext(Controller, User);
			var result = Controller.ResetPassword(string.Empty) as RedirectResult;
			result.Should().Not.Be.Null();
			result.Url.Should().Equal(Controller.Url.Home().Index());
			var notification = Controller.TempData[ViewDataConstants.Notification] as Notification;
			notification.Should().Not.Be.Null();
		}
		[Test]
		public void GivenAnonymousGetRequest_WithBadId_Redirects()
		{
			var result = Controller.ResetPassword("asdf") as RedirectResult;
			result.Should().Not.Be.Null();
			result.Url.Should().Equal(Controller.Url.Account().ForgotPassword());
			var notification = Controller.TempData[ViewDataConstants.Notification] as Notification;
			notification.Should().Not.Be.Null();
		}
		[Test]
		public void GivenAnonymousGetRequest_WithInvalidId_Redirects()
		{
			var result = Controller.ResetPassword(Guid.NewGuid().ToString("N")) as RedirectResult;
			result.Should().Not.Be.Null();
			result.Url.Should().Equal(Controller.Url.Account().ForgotPassword());
			var notification = Controller.TempData[ViewDataConstants.Notification] as Notification;
			notification.Should().Not.Be.Null();
		}
		[Test]
		public void GivenAnonymousGetRequest_WithValidId_ReturnsView()
		{
			var expectedObject = new PasswordRetrieval
				{
					Token = Guid.NewGuid(),
					UserId = User.Id
				};
			Db.Save(expectedObject);

			var result = Controller.ResetPassword(expectedObject.Token.ToString("N")) as ViewResult;
			result.Should().Not.Be.Null();
			var model = result.Model as ResetPassword;
			model.Should().Not.Be.Null();
			model.Token.Should().Equal(expectedObject.Token);
			model.Data.Should().Not.Be.Null();
			model.Data.Token.Should().Equal(expectedObject.Token);
			model.Data.UserId.Should().Equal(User.Id);
		}


		[Test]
		public void GivenAuthenticatedPostRequest_Redirects()
		{
			var model = new ResetPassword();
			ControllerUtilities.SetupControllerContext(Controller, User);
			var result = Controller.ResetPassword(model) as RedirectResult;
			result.Should().Not.Be.Null();
			result.Url.Should().Equal(Controller.Url.Home().Index());
			var notification = Controller.TempData[ViewDataConstants.Notification] as Notification;
			notification.Should().Not.Be.Null();
		}
		[Test]
		public void GivenAnonymousPostRequest_WithInvalidId_Redirects()
		{
			var model = new ResetPassword
				{
					Token = Guid.NewGuid()
				};
			var result = Controller.ResetPassword(model) as RedirectResult;
			result.Should().Not.Be.Null();
			result.Url.Should().Equal(Controller.Url.Account().ForgotPassword());
			var notification = Controller.TempData[ViewDataConstants.Notification] as Notification;
			notification.Should().Not.Be.Null();
		}
		[Test]
		public void GivenAnonymousPostRequest_WithValidData_ReturnsView()
		{
			var expectedObject = new PasswordRetrieval
				{
					Token = Guid.NewGuid(),
					UserId = User.Id
				};
			Db.Save(expectedObject);

			var model = new ResetPassword
				{
					Token = expectedObject.Token,
					Password = "Password11" + GetRandom.String(10),
				};
			model.PasswordConfirm = model.Password;
			var result = Controller.ResetPassword(model) as ViewResult;
			result.Should().Not.Be.Null();
			result.ViewName.Should().Equal("ResetPasswordConfirmation");
		}
		[Test]
		public void GivenAnonymousPostRequest_WithValidData_DeletesPasswordRetrieval()
		{
			var expectedObject = new PasswordRetrieval
				{
					Token = Guid.NewGuid(),
					UserId = User.Id
				};
			Db.Save(expectedObject);

			var model = new ResetPassword
				{
					Token = expectedObject.Token,
					Password = "Password11" + GetRandom.String(10),
				};
			model.PasswordConfirm = model.Password;
			var result = Controller.ResetPassword(model) as ViewResult;
			result.Should().Not.Be.Null();
			
			var previousObject = Db.SingleOrDefault<PasswordRetrieval>(new { expectedObject.Id });
			previousObject.Should().Be.Null();
		}
		[Test]
		public void GivenAnonymousPostRequest_WithValidData_UpdatesUserPassword()
		{
			var expectedObject = new PasswordRetrieval
				{
					Token = Guid.NewGuid(),
					UserId = User.Id
				};
			Db.Save(expectedObject);

			var model = new ResetPassword
				{
					Token = expectedObject.Token,
					Password = "Password11" + GetRandom.String(10),
				};
			model.PasswordConfirm = model.Password;
			Controller.ResetPassword(model);

			var user = Db.SingleOrDefault<User>(new { User.Id });
			user.Password.Should().Equal(model.PasswordConfirm.ToSHAHash());
		}
		[Test]
		public void GivenAnonymousPostRequest_WithValidData_LogsInUser()
		{
			var expectedObject = new PasswordRetrieval
				{
					Token = Guid.NewGuid(),
					UserId = User.Id
				};
			Db.Save(expectedObject);

			var model = new ResetPassword
				{
					Token = expectedObject.Token,
					Password = "Password11" + GetRandom.String(10),
				};
			model.PasswordConfirm = model.Password;
			Controller.ResetPassword(model);

			AuthenticationService.Verify(x => x.SetLoginCookie(It.Is<User>(u => u.Id == User.Id), true), Times.Once());
		}
		[Test]
		public void GivenAnonymousPostRequest_WithValidId_IncrementsMetric()
		{
			var expectedObject = new PasswordRetrieval
				{
					Token = Guid.NewGuid(),
					UserId = User.Id
				};
			Db.Save(expectedObject);

			var model = new ResetPassword
				{
					Token = expectedObject.Token,
					Password = "Password11" + GetRandom.String(10),
				};
			model.PasswordConfirm = model.Password;

			var result = Controller.ResetPassword(model) as ViewResult;
			result.Should().Not.Be.Null();
			MetricsMock.Verify(x => x.Increment(Metric.Users_ResetPassword), Times.Once());
		}

	}
}
