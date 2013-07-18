using System.Security.Principal;
using KickstartTemplate.Models.Users;

namespace KickstartTemplate.Infrastructure
{
	public class UserPrincipal : IPrincipal
	{
		public User UserObject { get; private set; }

		public UserPrincipal(User user, IIdentity identity)
		{
			UserObject = user;
			Identity = identity;
		}

		public IIdentity Identity { get; private set; }


		public bool IsInRole(string role)
		{
			// Not really needed in this app
			return true;
		}

		public bool IsAdmin
		{
			get
			{
				return UserObject != null && UserObject.IsAdmin;
			}
		}
	}
}