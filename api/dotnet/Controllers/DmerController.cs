using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dmft.Api.Services;
using Dmft.Api.Models;

namespace Dmft.Api.Controllers
{
    [ApiController]
    [Route("/api/queue/[controller]")]
    public class DmerController : ControllerBase
    {
        #region Variables
        private readonly ILogger<DmerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly MongoService _mongo;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a DmerController class.
        /// </summary>
        /// <param name="mongo"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public DmerController(MongoService mongo, ILogger<DmerController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _mongo = mongo;
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Adds a new DMER to the datasource.
        /// </summary>
        /// <param name="dmer"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddDmer([FromBody] object dmer)
        {
            var json = JsonSerializer.Serialize(dmer);
            var queue = _mongo.Add(json);

            return Created($"/api/dmer/{queue.Id}", queue.Id);
        }

        /// <summary>
        /// Get all the parcels for the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetToProcess()
        {
            var queue = _mongo.GetToProcess();
            return new JsonResult(queue);
        }

        /// <summary>
        /// Get the DMER for the specified Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var queue = _mongo.Get(id);
            return new JsonResult(queue);
        }

        /// <summary>
        /// Return an array of Queued items.
        /// </summary>
        /// <returns></returns>
        [HttpGet("list/{status}")]
        public IActionResult GetList(string status = "New")
        {
            var queue = _mongo.GetList(status);
            return new JsonResult(queue);
        }

        /// <summary>
        /// Update the status of the queued DMER.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        [HttpPut("processed")]
        public IActionResult Processed(Queue queue)
        {
            var result = _mongo.Processed(queue);

            return Ok("Success");
        }
        #endregion
    }
}
