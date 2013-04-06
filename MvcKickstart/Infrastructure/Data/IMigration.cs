using System.Data;

namespace MvcKickstart.Infrastructure.Data
{
	public interface IMigration
	{
		int Order { get; }
		void Execute(IDbConnection db);
	}
}