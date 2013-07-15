using AutoMapper;
using MvcKickstart.Models.Users;

namespace MvcKickstart
{
	public class AutomapperConfig
	{
		public static void CreateMappings()
		{
			Mapper.CreateMap<User, ViewModels.Account.Register>();
			Mapper.CreateMap<ViewModels.Account.Register, User>();
		}
	}
}