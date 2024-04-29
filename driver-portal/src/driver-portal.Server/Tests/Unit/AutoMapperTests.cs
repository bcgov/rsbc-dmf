﻿using AutoMapper;
using EnumsNET;
using Google.Protobuf.WellKnownTypes;
using Pssg.SharedUtils;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using Xunit;
using CaseDetail = Rsbc.Dmf.CaseManagement.Service.CaseDetail;
using Document = Rsbc.Dmf.DriverPortal.ViewModels.Document;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    public class AutoMapperTests
    {
        private readonly IMapper _mapper;
        private LegacyDocument _document;

        public AutoMapperTests(IMapper mapper)
        {
            _mapper = mapper;
            _document = new LegacyDocument
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
            var mappedDocument = _mapper.Map<Document>(_document);

            Assert.NotNull(mappedDocument);
            Assert.Equal("Received", mappedDocument.SubmittalStatus);
        }

        [Fact]
        public void Map_LegacyDocument_To_ViewModel_Document_With_Wrong_SubmittalStatus()
        {
            _document.SubmittalStatus = "Wrong SubmittalStatus";

            var mappedDocument = _mapper.Map<Document>(_document);

            Assert.NotNull(mappedDocument);
            Assert.Equal(_document.SubmittalStatus, mappedDocument.SubmittalStatus);
        }

        [Fact]
        public void Map_LegacyDocument_To_ViewModel_Document_With_Uploaded_SubmittalStatus()
        {
            _document.SubmittalStatus = SubmittalStatus.Uploaded.AsString();

            var mappedDocument = _mapper.Map<Document>(_document);

            Assert.NotNull(mappedDocument);
            Assert.Equal("Uploaded", mappedDocument.SubmittalStatus);
        }

        [Fact]
        public void Map_Callback_To_ViewModel()
        {
            var callback = new Callback();
            callback.Id = Guid.NewGuid().ToString();
            callback.RequestCallback = new Timestamp();
            callback.Subject = "Subject";
            callback.CallStatus = Callback.Types.CallbackCallStatus.Open;
            callback.ClosedDate = new Timestamp();

            var callbackViewModel = _mapper.Map<ViewModels.Callback>(callback);

            Assert.NotNull(callbackViewModel);
        }
    }
}
