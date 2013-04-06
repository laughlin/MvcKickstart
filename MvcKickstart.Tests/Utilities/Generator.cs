using System;
using System.Data;
using MvcKickstart.Infrastructure.Extensions;
using MvcKickstart.Models.Users;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace MvcKickstart.Tests.Utilities
{
	public class Generator
	{
		protected IDbConnection Db { get; set; }
		public Generator(IDbConnection db)
		{
			Db = db;
		}

		public User SetupUser(Action<User> modifications = null)
		{
			var item = Builder<User>.CreateNew()
						.With(x => x.Id = default(int))
						.And(x => x.Username = GetRandom.String(20))
						.And(x => x.Password = "password".ToSHAHash())
						.And(x => x.Email = GetRandom.Email())
						.Build();

			return ApplyModificationsAndSave(item, modifications);
		}

		private T ApplyModificationsAndSave<T>(T item, Action<T> modifications = null) where T : class
		{
			if (modifications != null)
			{
				modifications(item);
			}

			return Db.Save(item);
		}
	}
}
