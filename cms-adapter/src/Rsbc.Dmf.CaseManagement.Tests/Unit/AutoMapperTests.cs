using AutoMapper;
using Google.Protobuf.Collections;
using Rsbc.Dmf.CaseManagement.Dto;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
//using SharedUtils;
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

        [Fact]
        public void Map_CaseDetail()
        {
            var caseDetail = new CaseDetail();
            caseDetail.CaseSequence = 1;
            caseDetail.CaseId = "CaseId";
            caseDetail.DriverId = "DriverId";
            caseDetail.Title = "Title";
            caseDetail.IdCode = "IdCode";
            caseDetail.OpenedDate = DateTimeOffset.Now;
            caseDetail.CaseType = "CaseType";
            caseDetail.DmerType = "DmerType";
            caseDetail.Status = "Status";
            caseDetail.AssigneeTitle = "AssigneeTitle";
            caseDetail.LastActivityDate = DateTimeOffset.Now;
            caseDetail.DecisionDate = DateTimeOffset.Now;
            caseDetail.LatestDecision = "LatestDecision";
            caseDetail.DecisionForClass = "DecisionForClass";
            caseDetail.DpsProcessingDate = DateTimeOffset.Now;
            caseDetail.DriverLicenseNumber = "123344";
            caseDetail.FirstName = "Joe";
            caseDetail.LastName = "Trader";
            caseDetail.MiddleName = "Y";

            var mappedCaseDetail = _mapper.Map<Service.CaseDetail>(caseDetail);

            Assert.NotNull(mappedCaseDetail);
        }

        [Fact]
        public void Map_CaseManagement_DocumentSubType_To_Service_DocumentSubType()
        {
            var documentSubType = new DocumentSubType();
            documentSubType.Id = 1;
            documentSubType.Name = "Joe";

            var mappedDocumentSubType = _mapper.Map<Service.DocumentSubType>(documentSubType);

            Assert.NotNull(mappedDocumentSubType);
            Assert.Equal(documentSubType.Id, mappedDocumentSubType.Id);
            Assert.Equal(documentSubType.Name, mappedDocumentSubType.Name);
        }


        [Fact]
        public void Map_Service_Callback()
        {
            var callback = new Callback();
            callback.CaseId = Guid.NewGuid().ToString();
            callback.Id = Guid.NewGuid();
            callback.RequestCallback = new DateTimeOffset();
            callback.Subject = "Subject";
            callback.CallStatus = CallbackCallStatus.Open;
            callback.Closed = new DateTimeOffset();

            var mappedCallback = _mapper.Map<Service.Callback>(callback);

            Assert.NotNull(mappedCallback);
        }

        [Fact]
        public void Map_Callback_task_Serialize_Description()
        {
            var callback = new Callback();
            callback.Id = Guid.NewGuid();
            callback.RequestCallback = new DateTimeOffset();
            callback.Subject = "Subject";
            callback.Closed = new DateTimeOffset();
            callback.Assignee = "Assignee";
            callback.CallStatus = CallbackCallStatus.Open;
            callback.CaseId = Guid.NewGuid().ToString();
            //callback.Origin = (int)UserCode.Portal;
            callback.Origin = 100000005;
            callback.Phone = "Phone";
            callback.Priority = CallbackPriority.Low;
            callback.PreferredTime = PreferredTime.Morning;

            var entity = _mapper.Map<task>(callback);
            Assert.NotNull(entity);

            var deserializedCallback = _mapper.Map<Callback>(entity);
            Assert.NotNull(deserializedCallback);
            Assert.Equal("Phone", deserializedCallback.Phone);
            Assert.Equal(PreferredTime.Morning, deserializedCallback.PreferredTime);
        }

        [Fact]
        public void Map_bcgov_documenturl_To_CaseManagement_Document()
        {
            var document = new bcgov_documenturl();
            document.dfp_dmertype = 100000001;
            document.dfp_dmerstatus = 100000002;
            document.bcgov_CaseId = new incident();
            document.bcgov_CaseId.ticketnumber = "C123";
            document.bcgov_CaseId.customerid_contact = new contact();
            document.bcgov_CaseId.customerid_contact.fullname = "Joe Smithers";
            document.bcgov_CaseId.customerid_contact.firstname = "Joe";
            document.bcgov_CaseId.customerid_contact.lastname = "Smithers";
            document.bcgov_CaseId.customerid_contact.birthdate = new DateTime(2000, 1, 1);

            var mappedDocument = _mapper.Map<Document>(document);

            Assert.NotNull(mappedDocument);
            Assert.Equal("2 - Age", mappedDocument.DmerType);
            Assert.Equal("Submitted", mappedDocument.DmerStatus);
            Assert.Equal(document.bcgov_CaseId.ticketnumber, mappedDocument.Case.CaseNumber);
            Assert.Equal(document.bcgov_CaseId.customerid_contact.fullname, mappedDocument.Case.Person.FullName);
            Assert.Equal(document.bcgov_CaseId.customerid_contact.birthdate, mappedDocument.Case.Person.Birthday.Value);
        }

        [Fact]
        public void Map_CaseManagement_Document_To_Service_Document()
        {
            var document = new Document();
            document.DmerType = "2 - Age";
            document.DmerStatus = "Clean Pass";
            document.Case = new Dto.Case();
            document.Case.CaseNumber = "C123";
            document.Case.Person = new Person();
            document.Case.Person.FirstName = "Joe";
            document.Case.Person.LastName = "Smithers";
            document.Case.Person.Birthday = new DateTime(2000, 1, 1);
            document.Description = "Test Discription";
            document.ComplianceDate = DateTimeOffset.Now;
            document.DocumentType = new Dto.DocumentType();
            document.DocumentType.DocumentName = "DMER";
            document.DocumentSubType = new DocumentSubType();
            document.DocumentSubType.Name = "Sub type test";
            document.DocumentUrl = "DocumentUrl";
            document.SubmittalStatus = "Received";
            document.IdCode = "HG123";
            document.FaxReceivedDate = DateTimeOffset.Now;

            var mappedDocument = _mapper.Map<Service.Document>(document);

            Assert.NotNull(mappedDocument);
            Assert.Equal(document.DmerType, mappedDocument.DmerType);
            Assert.Equal(document.DmerStatus, mappedDocument.DmerStatus);
            Assert.Equal(document.Case.CaseNumber, mappedDocument.Case.CaseNumber);
            Assert.Equal(document.Case.Person.FullName, mappedDocument.Case.Person.FullName);
            Assert.Equal(document.Description, mappedDocument.Description);
            Assert.Equal(document.ComplianceDate, mappedDocument.ComplianceDate.ToDateTimeOffset());
            Assert.Equal(document.DocumentType.DocumentName, mappedDocument.DocumentType.DocumentName);
            Assert.Equal(document.DocumentSubType.Name, mappedDocument.DocumentSubType.Name);
            Assert.Equal(document.DocumentUrl, mappedDocument.DocumentUrl);
            Assert.Equal(document.SubmittalStatus, mappedDocument.SubmittalStatus);
            Assert.Equal(document.IdCode, mappedDocument.IdCode);
            Assert.Equal(document.FaxReceivedDate, mappedDocument.FaxReceivedDate.ToDateTimeOffset());
        }
    }
}