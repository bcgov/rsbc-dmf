using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System;
using NuGet.Frameworks;
using Rsbc.Dmf.CaseManagement.Dynamics;
using System.Reflection.Metadata;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using Org.BouncyCastle.Asn1.Ocsp;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics.Arm;
using System.Threading;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class DpsAddDocumentTests : WebAppTestBase
    {
        private readonly ICaseManager caseManager;

        public DpsAddDocumentTests(ITestOutputHelper output) : base(output)
        {           
            caseManager = services.GetRequiredService<ICaseManager>();            
        }

        /* Problematic routines
         * 
         * CaseManager.CreateLegacyCaseDocument
         * 
         */

        /*
         * Flow for DPS add document:
         * 1. GetDriver
         * 2. If Bypass Case Creation - 
         *      2.1 If LegacyDocument Type 
         *          2.1.1 CreateLegacyCaseDocument
         *          <DONE>
         *      2.2 If Unclassified or Remedial
         *          2.2.1 CreateDocumentOnDriver
         * 3 Else (Not bypass case creation)
         *      3.1 Search (Case ID) or Search (Drivers Licence) (depending on if CaseID provided)
         *      3.2 If PDR / Police
         *          3.2.1 CreateUnsolicitedCaseDocument
         *      3.3 Else (Not PDR / Police)
         *          3.3.1 CreateLegacyCaseDocument
         *      3.4 IF Clean pass document flag is set
         *          3.4.1 If Clean Pass
         *              4.4.1.1 UpdateCleanPassFlag
         *          3.4.2 If Manual Pass
         *              3.4.2.1 UpdateManualPassFlag
         *     <DONE>
         *          
         */

        [Fact(Skip = RequiresDynamics)]
        public async Task CanCreateLegacyCaseDocument()
        {
            string[] documentTypes = { 
                "Legacy Review",
                "Ignition Interlock Incident",
                "Ignition Interlock MIA",
                "RDP Registration",
                "Remedial Reconsideration",
                "Ignition Interlock Extension",
                "Ignition Interlock Reconsideration",
                "RDP and IIP Reconsideration",
                "Ignition Interlock Medical Exemption",
                "RDP Application For Extension",
                "High Risk Driving Incident Report",
                "Indefinite IIP",
                "OOP Certificate",
                "Client Letter Out DIP",
                "IIP Waiver",
                "OOP Document",
                "OOP Registration"
            };
            string[] documentTypeCodes = { 
                "LegacyReview",
                "080",
                "081",
                "110",
                "120",
                "121",
                "122",
                "123",
                "124",
                "250",
                "210",
                "125",
                "212",
                "320",
                "211",
                "213",
                "214"

            };

            for (int i = 0; i < documentTypes.Length; i++)
            {
                int caseSequence = 52222222 + i;

                string dl = "52222222";

                string temp = i.ToString();

                dl = dl.Substring(0, dl.Length - temp.Length) + temp;

                string name = $"TEST DRIVER {i}";

                var driverId = ((CaseManager) caseManager).AddDriver(name, dl);

                var caseId = ((CaseManager)caseManager).AddCase(driverId.Value, caseSequence);

                string documentUrl = $"TEST-DOCUMENT-{i}-{DateTime.Now.ToFileTimeUtc()}";

                // add a document

                LegacyDocument legacyDocumentRequest = new LegacyDocument
                {
                    BatchId = $"{i}",
                    CaseId = caseId.ToString(),
                    DocumentType = "Legacy Review",
                    DocumentTypeCode = "LegacyReview",
                    Driver = new Driver { DriverLicenseNumber = dl },
                    FaxReceivedDate = DateTimeOffset.UtcNow.AddDays(-1),
                    ImportDate = DateTimeOffset.UtcNow.AddDays(-1),
                    FileSize = 10,
                    DocumentPages = 1,
                    OriginatingNumber = "1",
                    SequenceNumber = 1,
                    DocumentUrl = documentUrl
                };

                await caseManager.CreateLegacyCaseDocument(legacyDocumentRequest);

                // confirm it is present

                var docs = await caseManager.GetCaseLegacyDocuments(caseId.ToString());

                bool found = false;

                string documentId = null;

                foreach (var doc in docs)
                {
                    if (doc.DocumentUrl == documentUrl)
                    {
                        found = true;
                        documentId = doc.DocumentId;
                        break;
                    }
                }

                Assert.True(found);

                // delete it

                await caseManager.DeleteLegacyDocument(documentId);

                // confirm that it is deleted

                found = false;

                docs = await caseManager.GetCaseLegacyDocuments(caseId.ToString());

                foreach (var doc in docs)
                {
                    if (doc.DocumentUrl == documentUrl)
                    {
                        found = true;
                        break;
                    }
                }

                Assert.False(found);

                // cleanup

                ((CaseManager)caseManager).DeleteCase(caseId.Value);
                ((CaseManager)caseManager).DeleteDriver(driverId.Value);



            }
        

        }
    
    }
}