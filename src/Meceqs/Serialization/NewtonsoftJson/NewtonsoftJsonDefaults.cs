using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Meceqs.Serialization.NewtonsoftJson
{
    public static class NewtonsoftJsonDefaults
    {
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
