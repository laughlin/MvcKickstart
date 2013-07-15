using System;

namespace MvcKickstart.Infrastructure.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class TimingMetricAttribute : Attribute
	{
	}
}