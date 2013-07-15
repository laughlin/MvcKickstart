using System.Web.Mvc;

namespace MvcKickstart.Areas.Admin
{
	public class AdminAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Admin";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			CreateMappings();
		}

		public static void CreateMappings()
		{
			// TODO: Put automapper mappings for objects in this area
		}
	}
}
