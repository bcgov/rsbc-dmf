using Rsbc.Dmf.CaseManagement.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IDocumentManager
    {
        Task<LegacyDocument> GetLegacyDocument(string documentId);
        Task<IEnumerable<LegacyDocument>> GetCaseLegacyDocuments(string caseId);
        Task<IEnumerable<LegacyDocument>> GetDriverLegacyDocuments(string driverLicenseNumber, bool includeEmpty);
        IEnumerable<Document> GetDocumentsByTypeForUsers(IEnumerable<Guid> loginIds, string documentTypeCode);
        Document GetDmer(Guid caseId);
        IEnumerable<Document> GetDriverAndCaseDocuments(string caseId, string loginId);

        IEnumerable<Document> UpdateClaimDmer(IEnumerable<Guid> loginIds, Guid driverId);

        IEnumerable<Document> UpdateUnClaimDmer(IEnumerable<Guid> loginIds, Guid driverId);

    }
}
