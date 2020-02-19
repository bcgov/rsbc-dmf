using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dmft.Api.Models
{
    public class Queue
    {
        #region Properties
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string Dmer { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ProcessOn { get; set; }

        public DateTime? ProcessedOn { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a Queue object.
        /// </summary>
        public Queue()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new instance of a Queue object, and initializes it.
        /// </summary>
        /// <param name="dmer"></param>
        public Queue(string dmer)
        {
            this.Id = Guid.NewGuid();
            this.Status = "New";
            this.Dmer = dmer;
            this.CreatedOn = DateTime.UtcNow;
        }
        #endregion
    }
}
