using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Dmft.Api.Models;
using Newtonsoft.Json.Linq;
using Dmft.Api.Helpers.Json;

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

            var db_domain = configuration["MONGODB_DOMAIN"];
            var db_port = configuration["MONGODB_PORT"];
            var db_name = configuration["MONGODB_DATABASE"];
            var db_user = configuration["MONGODB_USER"];
            var db_password = configuration["MONGODB_PASSWORD"];

            var cs = $"mongodb://{db_user}:{db_password}@{db_domain}:{db_port}/{db_name}";
            var client = new MongoClient(cs);
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

        public Queue Get(string id)
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

        /// <summary>
        /// Add the DMER object to the datasource.
        /// Extract the driver's license number and use it as the ID.
        /// </summary>
        /// <param name="dmer"></param>
        /// <returns></returns>
        public Queue Add(object dmer)
        {
            if (dmer == null) throw new ArgumentNullException(nameof(dmer));

            var json = JsonParser.Serialize(dmer);
            var id = JsonParser.GetDriverLicenseNumber(json);
            var queue = new Queue(id, json);
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
