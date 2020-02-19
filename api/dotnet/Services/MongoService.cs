using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Dmft.Api.Models;

namespace Dmft.Api.Services
{
    public class MongoService
    {
        #region Variables
        private readonly ILogger<MongoService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<Queue> _queue;
        #endregion

        #region Constructors
        public MongoService(IConfiguration configuration, ILogger<MongoService> logger)
        {
            _logger = logger;
            _configuration = configuration;

            var cs = configuration.GetConnectionString("Dmer");
            var client = new MongoClient(cs);

            var db_name = configuration["MONGODB_DATABASE"];
            var db = client.GetDatabase(db_name);

            _queue = db.GetCollection<Queue>("queue");
        }
        #endregion

        #region Methods
        public IEnumerable<Queue> GetList(string status = "New")
        {
            var result = _queue.Find<Queue>(q => q.Status == status);
            return result.ToList();
        }

        public Queue Get(Guid id)
        {
            return _queue.Find<Queue>(q => q.Id == id).FirstOrDefault();
        }

        public Queue GetFirstInQueue()
        {
            return _queue.Find<Queue>(q => q.Status == "New").FirstOrDefault();
        }

        public Queue GetToProcess()
        {
            var queue = GetFirstInQueue();

            if (queue != null)
            {
                queue.Status = "Processing";
                queue.ProcessOn = DateTime.UtcNow;

                Replace(queue);
            }
            return queue;
        }

        public Queue Processed(Queue queue)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));

            queue.Status = "Processed";
            queue.ProcessedOn = DateTime.UtcNow;
            Replace(queue);
            return queue;
        }

        public Queue Add(string dmer)
        {
            if (dmer == null) throw new ArgumentNullException(nameof(dmer));

            var queue = new Queue(dmer);
            _queue.InsertOne(queue);

            return queue;
        }

        public Queue Replace(Queue queue)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));

            _queue.ReplaceOne(q => q.Id == queue.Id, queue);

            return queue;
        }

        public void Remove(Queue queue)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));

            _queue.DeleteOne(q => q.Id == queue.Id);
        }
        #endregion
    }
}
