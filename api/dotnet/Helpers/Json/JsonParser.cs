using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Dmft.Api.Helpers.Json
{
    public static class JsonParser
    {
        public static string Serialize(object dmer)
        {
            return JsonSerializer.Serialize(dmer);
        }

        public static object Deserialize(string dmer)
        {
            return JsonSerializer.Deserialize<object>(dmer);
        }

        public static string GetDriverLicenseNumber(string dmer)
        {
            var jo = JObject.Parse(dmer);
            var patientDriversLicenseNumber = "patientDriversLicenseNumber";
            var token = jo.SelectToken($"$..item[?(@.linkId=='{patientDriversLicenseNumber}')].answer[0].valueDecimal");
            var id = token.Value<string>();
            return id;
        }
    }
}
