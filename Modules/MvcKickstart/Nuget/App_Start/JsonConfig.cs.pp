using ServiceStack.Text;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof($rootnamespace$.JsonConfig), "PreStart")]

namespace $rootnamespace$ 
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
