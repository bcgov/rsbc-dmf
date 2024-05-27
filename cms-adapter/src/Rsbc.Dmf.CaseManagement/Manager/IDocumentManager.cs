using Rsbc.Dmf.CaseManagement.DomainModels;
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

        IEnumerable<Document> GetDriverAndCaseDocuments(string caseId, string loginId);
    }
}
