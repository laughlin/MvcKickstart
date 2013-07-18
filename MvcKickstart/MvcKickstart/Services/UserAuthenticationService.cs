using System.Collections.Generic;
using System.Web.Security;
using KickstartTemplate.Models.Users;

namespace KickstartTemplate.Services
{
	public interface IUserAuthenticationService
	{
		void SetLoginCookie(User user, bool rememberMe);
		void Logout();
		IList<string> ReservedUsernames { get; }
	}

	public class UserAuthenticationService : IUserAuthenticationService
	{
		private static readonly IList<string> _reservedUsernames = new[] { "admin" };
		public IList<string> ReservedUsernames
		{
			get { return _reservedUsernames; }
		}

		public void SetLoginCookie(User user, bool rememberMe)
		{
			FormsAuthentication.SetAuthCookie(user.Username, rememberMe);
		}

		public void Logout()
		{
			FormsAuthentication.SignOut();
		}
	}
}