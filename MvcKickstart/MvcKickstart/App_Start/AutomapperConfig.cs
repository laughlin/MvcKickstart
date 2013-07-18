using AutoMapper;
using KickstartTemplate.Models.Users;

namespace KickstartTemplate
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