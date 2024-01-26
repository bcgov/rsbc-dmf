using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.DriverPortal.ViewModels;
using System;
using Xunit;
using CaseDetail = Rsbc.Dmf.CaseManagement.Service.CaseDetail;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class AutoMapperTests
    {
        private readonly IMapper _mapper;

        public AutoMapperTests(IMapper mapper)
        {
            _mapper = mapper;
        }

        [Fact]
        public void Map_Service_CaseDetail_To_ViewModel_CaseDetail()
        {
            var caseDetail = new CaseDetail();
            caseDetail.CaseId = "CaseId";
            caseDetail.Title = "Title";
            caseDetail.IdCode = "IdCode";
            caseDetail.CaseSequence = 1;
            caseDetail.OpenedDate = new Timestamp();
            caseDetail.CaseType = "CaseType";
            caseDetail.DmerType = "DmerType";
            caseDetail.Status = "Status";
            caseDetail.AssigneeTitle = "AssigneeTitle";
            caseDetail.LastActivityDate = new Timestamp();
            caseDetail.LatestDecision = "LatestDecision";
            caseDetail.DecisionForClass = "DecisionForClass";
            caseDetail.DecisionDate = new Timestamp();
            caseDetail.OutstandingDocuments = 1;
            caseDetail.DpsProcessingDate = new Timestamp();

            var mappedCaseDetail = _mapper.Map<ViewModels.CaseDetail>(caseDetail);

            Assert.NotNull(mappedCaseDetail);
            Assert.NotNull(mappedCaseDetail.DecisionDate);
        }

        [Fact]
        public void Map_LegacyDocument_To_ViewModel_Document()
        {
            var document = new LegacyDocument
            {
                ImportDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                DocumentId = "DocumentId",
                DocumentType = "DocumentType",
                DocumentTypeCode = "DocumentTypeCode",
                BusinessArea = "BusinessArea",
                SequenceNumber = 1,
                FaxReceivedDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                UserId = "UserId",
                CreateDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                DueDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                Description = "Description",
                DocumentUrl = "DocumentUrl",
                SubmittalStatus = "Clean Pass"
            };

            var mappedDocument = _mapper.Map<Document>(document);

            Assert.NotNull(mappedDocument);
            Assert.Equal("Received", mappedDocument.SubmittalStatus);
        }

        [Fact]
        public void Map_LegacyDocument_To_ViewModel_Document_With_Wrong_SubmittalStatus()
        {
            var document = new LegacyDocument
            {
                ImportDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                DocumentId = "DocumentId",
                DocumentType = "DocumentType",
                DocumentTypeCode = "DocumentTypeCode",
                BusinessArea = "BusinessArea",
                SequenceNumber = 1,
                FaxReceivedDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                UserId = "UserId",
                CreateDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                DueDate = Timestamp.FromDateTimeOffset(DateTimeOffset.MinValue),
                Description = "Description",
                DocumentUrl = "DocumentUrl",
                SubmittalStatus = "Wrong SubmittalStatus"
            };

            var mappedDocument = _mapper.Map<Document>(document);

            Assert.NotNull(mappedDocument);
            Assert.Equal(document.SubmittalStatus, mappedDocument.SubmittalStatus);
        }
    }
}
