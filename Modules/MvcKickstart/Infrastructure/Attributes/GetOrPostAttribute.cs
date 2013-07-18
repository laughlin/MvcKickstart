using System.Web.Mvc;
using AttributeRouting.Web.Mvc;

namespace MvcKickstart.Infrastructure.Attributes
{
	/// <summary>
	/// Defines a route for an action constrained to requests providing an httpMethod value of GET or POST.
	/// </summary>
	public class GetOrPostAttribute : RouteAttribute
	{
		/// <summary>
		/// Specify a route for an action constrained to requests providing an httpMethod value of GET or POST.
		/// </summary>
		/// <param name="routeUrl">The url that is associated with this action</param>
		public GetOrPostAttribute(string routeUrl) : base(routeUrl, HttpVerbs.Get, HttpVerbs.Post, HttpVerbs.Head)
		{
		}
	}
}