namespace MvcKickstart.Infrastructure
{
	public static class CacheKeys
	{
		public const string SiteSettings = "SiteSettings";

		public static class User
		{
			public static string ById(int userId)
			{
				return "User/ById/" + userId;
			}
			public static string ByUsername(string username)
			{
				return "User/ByUsername/" + username;
			}
		}
	}
}