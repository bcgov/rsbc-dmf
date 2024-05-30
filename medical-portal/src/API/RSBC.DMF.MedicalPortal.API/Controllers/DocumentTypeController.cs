using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsbc.Dmf.CaseManagement.Service;

namespace RSBC.DMF.MedicalPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentTypeController : Controller
    {
        private readonly DocumentManager.DocumentManagerClient _documentManagerClient;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public DocumentTypeController(DocumentManager.DocumentManagerClient documentManagerClient, IConfiguration configuration, IMapper mapper)
        {
            _documentManagerClient = documentManagerClient;
            _configuration = configuration;
            _mapper = mapper;
        }

        // get document sub types that are children of driver document type code that matchs configuration value
        [HttpGet("documentSubType")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.DocumentSubTypes>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ActionName(nameof(GetDriverDocumentSubTypes))]
        public async Task<ActionResult> GetDriverDocumentSubTypes()
        {
            // get driver document sub types
            var request = new DocumentTypeRequest();
            request.DocumentTypeCode = _configuration["DRIVER_DOCUMENT_TYPE_CODE"];
            var reply = _documentManagerClient.GetDocumentSubTypes(request);
            if (reply.ResultStatus != ResultStatus.Success)
            {
                return StatusCode(500, reply.ErrorDetail);
            }

            // return mapped results
            var result = _mapper.Map<IEnumerable<ViewModels.DocumentSubTypes>>(reply.Items);
            return Json(result);
        }
    }
}
