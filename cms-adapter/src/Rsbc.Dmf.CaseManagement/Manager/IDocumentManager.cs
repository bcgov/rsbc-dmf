using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface IDocumentManager
    {
        Task<LegacyDocument> GetLegacyDocument(string documentId);
        Task<IEnumerable<LegacyDocument>> GetCaseLegacyDocuments(string caseId);
        Task<IEnumerable<LegacyDocument>> GetDriverLegacyDocuments(string driverLicenseNumber, bool includeEmpty);
    }
}
