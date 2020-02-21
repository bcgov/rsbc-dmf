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
            var dlnPath = "patientDriversLicenseNumber";
            var token = jo.SelectToken($"$..item[?(@.linkId=='{dlnPath}')].answer[0].valueDecimal");
            var id = token.Value<string>();
            return id;
        }

        public static string GetDriverName(string dmer)
        {
            var jo = JObject.Parse(dmer);
            var familyNamePath = "patient.name.family";
            var givenNamePath = "patient.name.given";
            var familyName = jo.SelectToken($"$..item[?(@.linkId=='{familyNamePath}')].answer[0].valueString").Value<string>();
            var givenName = jo.SelectToken($"$..item[?(@.linkId=='{givenNamePath}')].answer[0].valueString").Value<string>();
            return $"{familyName}, {givenName}";
        }
    }
}
