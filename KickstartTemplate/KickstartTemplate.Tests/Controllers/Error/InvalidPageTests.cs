using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using KickstartTemplate.Controllers;
using KickstartTemplate.Infrastructure;
using KickstartTemplate.Tests.Utilities;
using NUnit.Framework;
using Should.Fluent;

namespace KickstartTemplate.Tests.Controllers.Error
{
	public class InvalidPageTests : TestBase
	{
		[Test]
		public void GivenRequest_ReturnsInvalidPageView()
		{
			var controller = new ErrorController(Db, Cache, Metrics);
			ControllerUtilities.SetupControllerContext(controller);

			var result = controller.InvalidPage() as ViewResult;
			result.Should().Not.Be.Null();
		}
		[Test]
		public void GivenRequest_Returns404HttpStatus()
		{
			var controller = new ErrorController(Db, Cache, Metrics);

			var request = new HttpRequest(string.Empty, "http://example.com/", string.Empty);
			var response = new HttpResponse(TextWriter.Null);
			var httpContext = new HttpContextWrapper(new HttpContext(request, response));
			controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

			controller.InvalidPage();

			controller.Response.StatusCode.Should().Equal((int) HttpStatusCode.NotFound);
		}
		[Test]
		public void GivenRequestWithQueryString_TracksMetric()
		{
			var controller = new ErrorController(Db, Cache, Metrics);

			var request = new HttpRequest(string.Empty, "http://example.com/", "404;http://example.com");
			var response = new HttpResponse(TextWriter.Null);
			var httpContext = new HttpContextWrapper(new HttpContext(request, response));
			controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

			controller.InvalidPage();

			MetricsMock.Verify(x => x.Increment(Metric.Error_404));
		}
		[Test]
		public void GivenRequestWithoutQueryString_DoesNotTrackMetric()
		{
			var controller = new ErrorController(Db, Cache, Metrics);

			var request = new HttpRequest(string.Empty, "http://example.com/", string.Empty);
			var response = new HttpResponse(TextWriter.Null);
			var httpContext = new HttpContextWrapper(new HttpContext(request, response));
			controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

			controller.InvalidPage();

			MetricsMock.Verify(x => x.Increment(Metric.Error_404), Times.Never());
		}
	}
}
