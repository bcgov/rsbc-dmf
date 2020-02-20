using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dmft.Api.Services;
using Dmft.Api.Models;
using System.Text.Json;
using MongoDB.Driver;

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
            try
            {
                var queue = _mongo.Add(dmer);
                return Created($"/api/dmer/{queue.Id}", queue.Id);
            }
            catch (MongoWriteException ex)
            {
                if (ex.Message.Contains("duplicate key error"))
                {
                    return Ok("Duplicate DMER submitted");
                }
                throw ex;
            }
        }

        /// <summary>
        /// Get all the parcels for the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetToProcess()
        {
            var queue = _mongo.GetToProcess();
            var dmer = JsonSerializer.Deserialize<object>(queue.Dmer);

            return new JsonResult(new
            {
                Id = queue.Id,
                Status = queue.Status,
                Dmer = dmer
            });
        }

        /// <summary>
        /// Get the DMER for the specified Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var queue = _mongo.Get(id);
            var dmer = JsonSerializer.Deserialize<object>(queue.Dmer);

            return new JsonResult(new
            {
                Id = queue.Id,
                Status = queue.Status,
                Dmer = dmer
            });
        }

        /// <summary>
        /// Return an array of Queued items.
        /// </summary>
        /// <returns></returns>
        [HttpGet("list/{status}")]
        public IActionResult GetList(string status = "New")
        {
            var queue = _mongo.GetList(status).Select(q =>
            {
                var dmer = JsonSerializer.Deserialize<object>(q.Dmer);
                return new
                {
                    Id = q.Id,
                    Status = q.Status,
                    Dmer = dmer
                };
            });
            return new JsonResult(queue);
        }

        /// <summary>
        /// Update the status of the queued DMER.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        [HttpPut("processed")]
        public IActionResult Processed(string id, string status)
        {
            var queue = _mongo.Get(id);
            if (queue == null) return BadRequest("DMER does not exist.");

            var result = _mongo.Processed(queue);

            return Ok("Success");
        }
        #endregion
    }
}
