using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Meceqs.Serialization.Json
{
    public static class JsonDefaults
    {
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}