using System.IO;

namespace MvcKickstart.Tests.Extensions
{
	public static class FileAndDirectoryExtensions
	{
		public static void Empty(this DirectoryInfo directory)
		{
			foreach (var file in directory.GetFiles()) 
				file.Delete();
			foreach (var dir in directory.GetDirectories())
				dir.Empty();

			directory.Delete(true);
		}
	}
}
