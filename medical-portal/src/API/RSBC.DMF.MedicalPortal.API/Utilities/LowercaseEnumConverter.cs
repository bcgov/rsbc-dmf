using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using RSBC.DMF.MedicalPortal.API.ViewModels;

namespace RSBC.DMF.MedicalPortal.API.Utilities
{
    public class LowercaseEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ChefsSubmission);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var dto = new ChefsSubmission
            {
                Status = SubmissionStatus.Final,
                Submission = new Dictionary<string, object>()
            };

            if (jsonObject.TryGetValue("Status", out JToken statusToken))
            {
                dto.Status = statusToken.ToObject<SubmissionStatus>(serializer);
            }

            if (jsonObject.TryGetValue("Submission", out JToken submissionToken))
            {
                var submissionObject = (JObject)submissionToken;
                foreach (var property in submissionObject.Properties())
                {
                    dto.Submission[property.Name] = property.Value.ToObject<object>();
                }
            }

            return dto;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because we don't need serialization");
        }
    }
}