using Google.Protobuf.WellKnownTypes;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.DriverPortal.Api
{
    public class DocumentFactory
    {
        public LegacyDocument Create(Driver driver, string userId, string caseId = "")
        {
            var importDate = DateTimeOffset.Now;
            var faxReceivedDate = DateTimeOffset.Now;
            var pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            if (importDate.Offset == TimeSpan.Zero)
            {
                importDate = TimeZoneInfo.ConvertTimeToUtc(importDate.DateTime, pacificZone);
            }
            if (faxReceivedDate.Offset == TimeSpan.Zero)
            {
                faxReceivedDate = TimeZoneInfo.ConvertTimeToUtc(faxReceivedDate.DateTime, pacificZone);
            }

            return new LegacyDocument()
            {
                CaseId = caseId,
                SequenceNumber = 0,
                UserId = userId,
                FaxReceivedDate = Timestamp.FromDateTimeOffset(faxReceivedDate),
                ImportDate = Timestamp.FromDateTimeOffset(importDate),
                Driver = driver,

                // TODO if you want to use this function in a more generic way
                // use CaseManager.GetDocumentType to populate the following 3 lines
                DocumentTypeCode = "001",
                DocumentType = "DMER",
                BusinessArea = "Driver Fitness",

                Priority = string.Empty,
                Owner = string.Empty,
                BatchId = string.Empty,
                ValidationMethod = string.Empty,
                ValidationPrevious = string.Empty,
                ImportId = string.Empty,
                OriginatingNumber = string.Empty,
                SubmittalStatus = "Sent",
            };
        }
    }
}
