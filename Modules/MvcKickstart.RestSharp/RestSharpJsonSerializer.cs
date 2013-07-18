using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace MvcKickstart.RestSharp
{
	public class RestSharpJsonSerializer : ISerializer, IDeserializer
	{
		public RestSharpJsonSerializer()
		{
			ContentType = "application/json";
		}

		public string Serialize(object obj)
		{
			return ServiceStack.Text.JsonSerializer.SerializeToString(obj);
		}

		public T Deserialize<T>(IRestResponse response)
		{
			return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(response.Content);
		}

		public string RootElement { get; set; }
		public string Namespace { get; set; }
		public string DateFormat { get; set; }
		public string ContentType { get; set; }
	}
}