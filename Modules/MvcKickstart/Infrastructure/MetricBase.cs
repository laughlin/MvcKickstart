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

		public const string Error_Unhandled = "Errors.Unhandled";
	}
}
