using AutoMapper;
using Google.Protobuf.Collections;
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
    }
}