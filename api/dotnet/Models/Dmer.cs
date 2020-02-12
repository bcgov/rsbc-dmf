using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dmft.Api.Models
{
    public class Dmer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
