using KickstartTemplate.Areas.Admin;
using NUnit.Framework;

namespace KickstartTemplate.Tests.Controllers.Admin
{
	[SetUpFixture]
	public class AdminAreaTests
	{
		[SetUp]
		public void Setup()
		{
			AdminAreaRegistration.CreateMappings();
		}
	}
}
