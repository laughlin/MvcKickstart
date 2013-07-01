using ServiceStack.Text;

namespace MvcKickstart
{
	public class JsonConfig
	{
		public static void Setup()
		{
			JsConfig.EmitCamelCaseNames = true;
			JsConfig.AlwaysUseUtc = true;
			JsConfig.DateHandler = JsonDateHandler.ISO8601;
			JsConfig.ExcludeTypeInfo = true;
		}
	}
}