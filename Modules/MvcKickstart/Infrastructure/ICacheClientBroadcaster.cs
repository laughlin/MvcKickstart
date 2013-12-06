namespace MvcKickstart.Infrastructure
{
	public interface ICacheClientBroadcaster
	{
		bool Remove(string key, bool broadcast);
		void FlushAll(bool broadcast);
	}
}
