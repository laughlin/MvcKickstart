using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;

namespace KickstartTemplate.Tests.Extensions
{
	public static class RouteTestingExtensions
	{
		public static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, Task<ActionResult>>> action) where TController : Controller
		{
			routeData.ShouldNotBeNull("The URL did not match any route");

			//check controller
			routeData.ShouldMapTo<TController>();

			//check action
			var methodCall = (MethodCallExpression) action.Body;
			string actualAction = routeData.Values.GetValue("action").ToString();
			string expectedAction = methodCall.Method.Name;
			actualAction.AssertSameStringAs(expectedAction);

			//check parameters
			for (int i = 0; i < methodCall.Arguments.Count; i++)
			{
				string name = methodCall.Method.GetParameters()[i].Name;
				object value = null;

				switch (methodCall.Arguments[i].NodeType)
				{
					case ExpressionType.Constant:
						value = ((ConstantExpression) methodCall.Arguments[i]).Value;
						break;

					case ExpressionType.MemberAccess:
						value = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
						break;

				}

				value = (value == null ? value : value.ToString());
				routeData.Values.GetValue(name).ShouldEqual(value, "Value for parameter did not match");
			}

			return routeData;
		}

		public static RouteData ShouldMapTo<TController>(this string relativeUrl, Expression<Func<TController, Task<ActionResult>>> action) where TController : Controller
		{
			return relativeUrl.Route().ShouldMapTo(action);
		}

	}
}
