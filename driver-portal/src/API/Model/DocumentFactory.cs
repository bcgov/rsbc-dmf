﻿using EnumsNET;
using Google.Protobuf.WellKnownTypes;
using Pssg.SharedUtils;
using Rsbc.Dmf.CaseManagement.Service;

namespace Rsbc.Dmf.DriverPortal.Api
{
    public class DocumentFactory
    {
        public LegacyDocument Create(Driver driver, string userId, string documentUrl, string documentType, string documentTypeCode, string caseId = "")
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
                DocumentTypeCode = documentTypeCode,
                DocumentType = documentType,
                BusinessArea = "",
                DocumentUrl = documentUrl,
                Priority = string.Empty,
                // maps to "Intake Team"
                Owner = "Client Services",
                Origin = "Driver Portal",
                BatchId = string.Empty,
                ValidationMethod = string.Empty,
                ValidationPrevious = string.Empty,
                ImportId = string.Empty,
                OriginatingNumber = string.Empty,
                SubmittalStatus = SubmittalStatus.Uploaded.AsString(),
            };
        }
    }
}
