using System.ComponentModel.DataAnnotations;
using MvcKickstart.Infrastructure.Attributes;

namespace MvcKickstart.Infrastructure
{
	/// <summary>
	/// An enumeration of all internal metrics tracked in the system
	/// </summary>
	/// <remarks>
	/// Optionally specify the name (DisplayAttribute) to give a more descriptive name to the metric
	/// Optionally specify the group name (DisplayAttribute), to group similar metrics.
	/// </remarks>
	public enum Metric
	{
		#region Users

		[Display(Name = "SendPasswordResetEmail", GroupName = "Users")]
		Users_SendPasswordResetEmail,
		[Display(Name = "ResetPassword", GroupName = "Users")]
		Users_ResetPassword,
		[Display(Name = "ChangePassword", GroupName = "Users")]
		Users_ChangePassword,
		[Display(Name = "FailedLogin", GroupName = "Users")]
		Users_FailedLogin,
		[Display(Name = "SuccessfulLogin", GroupName = "Users")]
		Users_SuccessfulLogin,
		[Display(Name = "Logout", GroupName = "Users")]
		Users_Logout,
		[Display(Name = "Register", GroupName = "Users")]
		Users_Register,

		#endregion

		#region Errors
		[Display(Name = "Fatal", GroupName = "Errors")]
		Error_Fatal,
		[Display(Name = "Warn", GroupName = "Errors")]
		Error_Warn,
		[Display(Name = "404", GroupName = "Errors")]
		Error_404,
		#endregion
	}
}