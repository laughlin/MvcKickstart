using MvcKickstart.Infrastructure;
using NUnit.Framework;
using Should.Fluent;

namespace KickstartTemplate.Tests.Controllers.Account
{
	public class ValidateUsernameTests : ControllerTestBase
	{
		public override void SetupFixture()
		{
			base.SetupFixture();

			Generator.SetupUser(x =>
				{
					x.Username = "TestUser";
				});
			Generator.SetupUser(x =>
				{
					x.Username = "TestUserDeleted";
					x.IsDeleted = true;
				});
		}

		[Test]
		public void GivenRequest_ReturnsJson()
		{
			var result = Controller.ValidateUsername("TestUser") as ServiceStackJsonResult;
			result.Should().Not.Be.Null();
			result.Data.Should().Not.Be.Null();
			result.Data.Should().Be.OfType<bool>();
		}

		[Test]
		public void GivenReservedUsername_ReturnsFalse()
		{
			var result = Controller.ValidateUsername("Admin") as ServiceStackJsonResult;
			result.Should().Not.Be.Null();
			((bool) result.Data).Should().Be.False();
		}

		[Test]
		public void GivenExistingUsername_ReturnsFalse()
		{
			var result = Controller.ValidateUsername("TestUser") as ServiceStackJsonResult;
			result.Should().Not.Be.Null();
			((bool) result.Data).Should().Be.False();
		}

		[Test]
		public void GivenExistingUsername_IgnoresUsernameCase_ReturnsFalse()
		{
			var result = Controller.ValidateUsername("testuser") as ServiceStackJsonResult;
			result.Should().Not.Be.Null();
			((bool) result.Data).Should().Be.False();
		}

		[Test]
		public void GivenExistingDeletedUsername_ReturnsFalse()
		{
			var result = Controller.ValidateUsername("TestUserDeleted") as ServiceStackJsonResult;
			result.Should().Not.Be.Null();
			((bool) result.Data).Should().Be.False();
		}

		[Test]
		public void GivenNonExistingUsername_ReturnsTrue()
		{
			var result = Controller.ValidateUsername("nonExistantUser") as ServiceStackJsonResult;
			result.Should().Not.Be.Null();
			((bool) result.Data).Should().Be.True();
		}
	}
}
