using System.Web.Mvc;
using KickstartTemplate.ViewModels.Home;
using NUnit.Framework;
using Should.Fluent;

namespace KickstartTemplate.Tests.Controllers.Home
{
	public class IndexTests : ControllerTestBase
	{
		[Test]
		public void GivenRequest_ReturnsView()
		{
			var result = Controller.Index() as ViewResult;
			result.Should().Not.Be.Null();
			result.ViewName.Should().Be.NullOrEmpty();
			result.Model.Should().Be.OfType<Index>();
		}
	}
}
