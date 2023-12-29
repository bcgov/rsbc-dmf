using AutoMapper;
using System;
using Xunit;

namespace Rsbc.Dmf.CaseManagement.Tests.Unit
{
    public class AutoMapperTests
    {
        private readonly IMapper _mapper;

        public AutoMapperTests(IMapper mapper)
        {
            _mapper = mapper;
        }

        [Fact]
        public void Map_LegacyDocument()
        {
            var document = new LegacyDocument();
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

            var mappedDocument = _mapper.Map<Service.LegacyDocument>(document);

            Assert.NotNull(mappedDocument);
        }
    }
}