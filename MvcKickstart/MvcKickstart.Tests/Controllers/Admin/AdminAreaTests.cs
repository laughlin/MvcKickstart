using System;
using MvcKickstart.Areas.Admin;
using NUnit.Framework;

namespace MvcKickstart.Tests.Controllers.Admin
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
