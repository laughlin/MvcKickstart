using ServiceStack.Text;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(KickstartTemplate.JsonConfig), "PreStart")]

namespace KickstartTemplate 
{
	public static class JsonConfig
	{
		public static void PreStart() 
		{
			JsConfig.EmitCamelCaseNames = true;
			JsConfig.AlwaysUseUtc = true;
			JsConfig.DateHandler = JsonDateHandler.ISO8601;
			JsConfig.ExcludeTypeInfo = true;
		}
	}
}
