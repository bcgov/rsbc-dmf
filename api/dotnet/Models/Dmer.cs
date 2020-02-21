using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Dmft.Api.Helpers.Json;

namespace Dmft.Api.Models
{
    public class Dmer
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public Dmer() { }

        public Dmer(Queue queue)
        {
            this.Id = queue.Id;
            this.Status = queue.Status;
            this.Name = JsonParser.GetDriverName(queue.Dmer);
        }
    }
}
