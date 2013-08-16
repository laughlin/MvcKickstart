using MvcKickstart.Infrastructure;

namespace KickstartTemplate.Infrastructure
{
	/// <summary>
	/// A list of all internal metrics tracked in the system
	/// </summary>
	public class Metric : MetricBase
	{
		#region Users

		public const string Users_SendPasswordResetEmail = "Users.SendPasswordResetEmail";
		public const string Users_ResetPassword = "Users.ResetPassword";
		public const string Users_ChangePassword = "Users.ChangePassword";
		public const string Users_FailedLogin = "Users.FailedLogin";
		public const string Users_SuccessfulLogin = "Users.SuccessfulLogin";
		public const string Users_Logout = "Users.Logout";
		public const string Users_Register = "Users.Register";

		#endregion
	}
}