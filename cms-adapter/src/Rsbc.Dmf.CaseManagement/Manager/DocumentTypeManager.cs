using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
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
        Guid GetDocumentSubTypeGuid(int id, string documentTypeCode);
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

        public Guid GetDocumentSubTypeGuid(int id, string documentTypeCode)
        {
            var documentSubTypes = GetDynamicsDocumentSubTypes(documentTypeCode).ToList();
            var mappedDocumentSubTypes = MapDocumentSubTypeId(documentSubTypes).ToList();
            var documentSubTypeIndex = mappedDocumentSubTypes.FindIndex(dst => dst.Id == id);
            return documentSubTypes[documentSubTypeIndex].dfp_documentsubtypeid.Value;
        }

        public IEnumerable<DocumentSubType> GetDocumentSubTypes(string documentTypeCode)
        {
            var documentSubTypes = GetDynamicsDocumentSubTypes(documentTypeCode);
            return MapDocumentSubTypeId(documentSubTypes);
        }

        private IEnumerable<dfp_documentsubtype> GetDynamicsDocumentSubTypes(string documentTypeCode)
        {
            var documentSubTypes = _dynamicsContext.dfp_documentsubtypes
                .Expand(dst => dst.dfp_DocumentTypeID)
                .Where(dst => dst.dfp_DocumentTypeID.dfp_code == documentTypeCode)
                .OrderBy(dst => dst.dfp_name)
                // to guarantee the same order even when names are the same
                .OrderBy(dst => dst.dfp_documentsubtypeid)
                .ToList();

            return documentSubTypes;
        }

        private IEnumerable<DocumentSubType> MapDocumentSubTypeId(IEnumerable<dfp_documentsubtype> documentSubTypes)
        {
            var mappedDocumentSubTypes = _mapper.Map<IEnumerable<DocumentSubType>>(documentSubTypes);
            int id = 0;
            foreach (var documentSubType in mappedDocumentSubTypes)
            {
                documentSubType.Id = id++;
            }

            return mappedDocumentSubTypes;
        }
    }
}
