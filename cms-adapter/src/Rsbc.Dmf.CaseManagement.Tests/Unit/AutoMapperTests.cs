using AutoMapper;
using Google.Protobuf.Collections;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using Xunit;

namespace Rsbc.Dmf.CaseManagement.Tests.Unit
{
    public class AutoMapperTests
    {
        private readonly IMapper _mapper;
        private readonly LegacyDocument document;

        public AutoMapperTests(IMapper mapper)
        {
            _mapper = mapper;

            document = new LegacyDocument();
            document.BatchId = "BatchId";
            document.BusinessArea = "BusinessArea";
            document.CaseId = "CaseId";
            document.DocumentPages = 1;
            document.DocumentId = "DocumentId";
            document.DocumentType = "DocumentType";
            document.DocumentTypeCode = "DocumentTypeCode";
            document.DocumentUrl = "DocumentUrl";
            document.ImportId = "ImportId";
            document.OriginatingNumber = "OriginatingNumber";
            document.ValidationMethod = "ValidationMethod";
            document.ValidationPrevious = "ValidationPrevious";
            document.SequenceNumber = 1;
            document.SubmittalStatus = "SubmittalStatus";
            document.CreateDate = DateTimeOffset.Now;
            document.Description = "Description";

            var driver = new Driver();
            driver.DriverLicenseNumber = "DriverLicenseNumber";
            driver.Surname = "Surname";
            driver.BirthDate = DateTime.Now;
            document.Driver = driver;
        }

        [Fact]
        public void Map_Service_LegacyDocument()
        {
            var mappedDocument = _mapper.Map<Service.LegacyDocument>(document);

            Assert.NotNull(mappedDocument);
        }

        [Fact]
        public void Map_Service_LegacyDocuments()
        {
            var documents = new List<LegacyDocument>();
            for (int i = 0; i < 10; i++)
            {
                documents.Add(document);
            }

            var mappedDocuments = _mapper.Map<RepeatedField<LegacyDocument>>(documents);

            Assert.NotNull(mappedDocuments);
        }

        [Fact]
        public void Map_Dynamics_LegacyDocument()
        {
            var documentType = new dfp_submittaltype();
            documentType.dfp_name = "DocumentType";
            documentType.dfp_apidocumenttype = "DocumentTypeCode";
            documentType.dfp_businessarea = 100000000;

            var document = new bcgov_documenturl();
            document.dfp_batchid = "BatchId";
            document.dfp_documentpages = "1";
            document.bcgov_documenturlid = Guid.NewGuid();
            document.dfp_DocumentTypeID = documentType;
            document.bcgov_url = "DocumentUrl";
            document.dfp_importid = "ImportId";
            document.dfp_dpsprocessingdate = new DateTimeOffset();
            document.dfp_faxreceiveddate = new DateTimeOffset();
            document.dfp_faxsender = "OriginatingNumber";
            document.dfp_validationmethod = "ValidationMethod";
            document.dfp_validationprevious = "ValidationPrevious";
            document.dfp_submittalstatus = 1;

            var mappedDocument = _mapper.Map<LegacyDocument>(document);

            Assert.NotNull(mappedDocument);
        }
    }
}