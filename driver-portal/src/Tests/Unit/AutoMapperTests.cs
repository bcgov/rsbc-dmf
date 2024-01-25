using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Rsbc.Dmf.CaseManagement.Service;
using Xunit;

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
        public void Map_ViewModel_CaseDetail_To_Service_CaseDetail()
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

            var mappedCaseDetail = _mapper.Map<CaseDetail, ViewModels.CaseDetail>(caseDetail);

            Assert.NotNull(mappedCaseDetail);
            Assert.NotNull(mappedCaseDetail.DecisionDate);
        }
    }
}
