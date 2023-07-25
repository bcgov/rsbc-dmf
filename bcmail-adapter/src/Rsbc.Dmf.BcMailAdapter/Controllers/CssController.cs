using DocumentFormat.OpenXml.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Interfaces;
using Rsbc.Interfaces.CdgsModels;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using JsonSerializer = System.Text.Json.JsonSerializer;
using PaperKind = WkHtmlToPdfDotNet.PaperKind;

namespace Rsbc.Dmf.BcMailAdapter.Controllers
{
    /// <summary>
    /// Controller providing data related to a Driver
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CssController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<CssController> _logger;        
        private readonly IConverter Converter;

        private readonly CssManager.CssManagerClient _cssManagerClient;
        private readonly IMemoryCache _cache;

   
            




            /// <summary>
            /// 
            /// </summary>
            /// <param name="logger"></param>
            /// <param name="configuration"></param>
            /// <param name="cdgsClient"></param>
            public CssController(ILogger<CssController> logger, CssManager.CssManagerClient cssManagerClient, IConfiguration configuration, IMemoryCache memoryCache)
        {
            Configuration = configuration;
            _logger = logger;
            _cache = memoryCache;
            _cssManagerClient = cssManagerClient;
        }


        /// <summary>
        /// Get Css
        /// </summary>
        /// <returns></returns>

        // GET: /Css/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [AllowAnonymous]

        public ActionResult GetCss(Guid id)  

        {
            var result = _cssManagerClient.GetCss(new CssRequest() { Id = id.ToString() });
            string css = null;
            if (result.ResultStatus == CaseManagement.Service.ResultStatus.Success)
            {
                css = result.Css;
            }
            return base.Content(css, "text/css");
        }



      

    }
}
