using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Dynamics;
using System.Collections.Generic;
using System.Linq;

namespace Rsbc.Dmf.CaseManagement
{
    // DOMAIN document types, document sub types, submittal types

    public class DocumentSubType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface IDocumentTypeManager
    {
        IEnumerable<DocumentSubType> GetDocumentSubTypes(string documentTypeCode);
    }

    internal class DocumentTypeManager : IDocumentTypeManager
    {
        internal readonly DynamicsContext _dynamicsContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentTypeManager> _logger;
        private readonly IMapper _mapper;

        public DocumentTypeManager(DynamicsContext dynamicsContext, IConfiguration configuration, ILogger<DocumentTypeManager> logger, IMapper mapper)
        {
            _dynamicsContext = dynamicsContext;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<DocumentSubType> GetDocumentSubTypes(string documentTypeCode)
        {
            var documentSubTypes = _dynamicsContext.dfp_documentsubtypes
                .Expand(dt => dt.dfp_DocumentTypeID)
                .Where(dst => dst.dfp_DocumentTypeID.dfp_code == documentTypeCode)
                .ToList();
            return _mapper.Map<IEnumerable<DocumentSubType>>(documentSubTypes);
        }
    }
}
