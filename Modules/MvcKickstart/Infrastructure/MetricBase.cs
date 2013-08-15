using MvcKickstart.Infrastructure.Attributes;

namespace MvcKickstart.Infrastructure
{
	public class MetricBase
	{
		public const string Users_RequestAuthenticated = "Users.RequestAuthenticated";
		public const string Users_RequestAnonymous = "Users.RequestAnonymous";

		[TimingMetric]
		public const string Profiling_RenderTime = "Profiling.RenderTime";
		[TimingMetric]
		public const string Profiling_ResolveRoute = "Profiling.ResolveRoute";

		public const string Error_404 = "Errors.404";
		public const string Error_Fatal = "Errors.Fatal";
		public const string Error_Error = "Errors.Error";
		public const string Error_Warn = "Errors.Warn";
		public const string Error_Info = "Errors.Info";
		public const string Error_Unhandled = "Errors.Unhandled";
	}
}
