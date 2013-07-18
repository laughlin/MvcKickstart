namespace MvcKickstart.Infrastructure
{
	public abstract class VaryByCustom
	{
		/// <summary>
		/// Each user should have their own cached copy
		/// </summary>
		public const string User = "user";
		/// <summary>
		/// Only vary based on if the user is authenticated or not.
		/// </summary>
		public const string UserIsAuthenticated = "user_or_anonymous";
		/// <summary>
		/// Vary based on if it's an ajax request or not
		/// </summary>
		public const string Ajax = "ajax";
	}
}