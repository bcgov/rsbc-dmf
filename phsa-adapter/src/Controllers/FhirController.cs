using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rsbc.Dmf.Interfaces.IcbcAdapter;
using Rsbc.Dmf.PhsaAdapter.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Hl7.Fhir.ElementModel;
using Pssg.DocumentStorageAdapter;
using Pssg.Rsbc.Dmf.DocumentTriage;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Dmf.Interfaces.IcbcAdapter.Models;
using UploadFileRequest = Pssg.DocumentStorageAdapter.UploadFileRequest;
using Rsbc.Dmf.PhsaAdapter.Extensions;
using Rsbc.Dmf.PhsaAdapter.Models;

namespace Rsbc.Dmf.PhsaAdapter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize("OAuth")]
    public class FhirController : ControllerBase
    {
        private readonly ILogger<ReceiveController> _logger;
        private readonly IConfiguration Configuration;
        private readonly IStructureDefinitionSummaryProvider _provider = new PocoStructureDefinitionSummaryProvider();
        private readonly IIcbcClient _icbcClient;
        private readonly CaseManager.CaseManagerClient _cmsAdapterClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;
        private readonly DocumentTriage.DocumentTriageClient _documentTriageClient;

        public FhirController(ILogger<ReceiveController> logger, IIcbcClient icbcClient, IConfiguration configuration,
            CaseManager.CaseManagerClient cmsAdapterClient,
            DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient,
            DocumentTriage.DocumentTriageClient documentTriageClient
            )
        {
            _logger = logger;
            Configuration = configuration;
            _icbcClient = icbcClient;
            _cmsAdapterClient = cmsAdapterClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
            _documentTriageClient = documentTriageClient;
        }

        [HttpGet("metadata")]
        [AllowAnonymous]
        public FhirResponse GetMetaData()
        {
            CapabilityStatement capabilityStatement = new CapabilityStatement()
            {
                Date = DateTimeOffset.Now.ToString(),
                Kind = CapabilityStatementKind.Instance,
                FhirVersion = FHIRVersion.N4_0_0,
                Software = new CapabilityStatement.SoftwareComponent() { Name = "RSBC PHSA Adapter" },
                Status = PublicationStatus.Active,
                Format = new List<string>() { "application/fhir+json" },
                Implementation = new CapabilityStatement.ImplementationComponent()
                {
                    Description = "RSBC FHIR",
                    Url = Configuration["FHIR_SERVER_URL"]
                },
                Rest = new List<CapabilityStatement.RestComponent>()
                {
                    new CapabilityStatement.RestComponent()
                    {
                        Mode = CapabilityStatement.RestfulCapabilityMode.Server,
                        Resource = new List<CapabilityStatement.ResourceComponent>()
                        {
                            new CapabilityStatement.ResourceComponent()
                            {
                                Type = ResourceType.Patient,
                                Profile = "http://hl7.org/fhir/StructureDefinition/Patient",
                                Interaction = new List<CapabilityStatement.ResourceInteractionComponent>()
                                {
                                    new CapabilityStatement.ResourceInteractionComponent()
                                        {Code = CapabilityStatement.TypeRestfulInteraction.Read}
                                }
                            },
                            new CapabilityStatement.ResourceComponent()
                            {
                                Type = ResourceType.Practitioner,
                                Profile = "http://hl7.org/fhir/StructureDefinition/Practitioner",
                                Interaction = new List<CapabilityStatement.ResourceInteractionComponent>()
                                {
                                    new CapabilityStatement.ResourceInteractionComponent()
                                        {Code = CapabilityStatement.TypeRestfulInteraction.Read}
                                }
                            },
                            new CapabilityStatement.ResourceComponent()
                            {
                                Type = ResourceType.Bundle,
                                Profile = "http://hl7.org/fhir/StructureDefinition/Bundle",
                                Interaction = new List<CapabilityStatement.ResourceInteractionComponent>()
                                {
                                    new CapabilityStatement.ResourceInteractionComponent()
                                        {Code = CapabilityStatement.TypeRestfulInteraction.Read},
                                    new CapabilityStatement.ResourceInteractionComponent()
                                        {Code = CapabilityStatement.TypeRestfulInteraction.Update}
                                }
                            }
                        }
                    }
                }
            };
            var result = Respond.WithResource(capabilityStatement);
            return result;
        }

        [HttpGet(".well-known/smart-configuration")]
        [AllowAnonymous]
        public IActionResult GetSmartConfiguration()
        {
            Response.ContentType = "application/json";
            List<string> capabilities = new List<string>();
            capabilities.Add("launch-ehr");
            capabilities.Add("client-public");
            capabilities.Add("client-confidential-symmetric");
            capabilities.Add("context-ehr-patient");
            capabilities.Add("sso-openid-connect");
            SmartConfiguration result = new SmartConfiguration()
            {
                Authorization_endpoint = Configuration["FHIR_AUTHORIZATION_ENDPOINT"],
                Token_endpoint = Configuration["FHIR_TOKEN_ENDPOINT"],
                Introspection_endpoint = Configuration["FHIR_INTROSPECTION_ENDPOINT"],
                Capabilities = capabilities
            };
            return new JsonResult(result);
        }

        [HttpGet("Patient/{id}")]
        [AllowAnonymous]
        public IActionResult GetPatient([FromRoute] string id)
        {
            Patient result = new Patient()
            {
                Id = id,
                Address = new List<Hl7.Fhir.Model.Address>()
                {
                    new Hl7.Fhir.Model.Address()
                    {
                        Line = new List<string>(){"123 Main Street"},
                        State = "BC1"
                    }
                },
                Name = new List<HumanName>(){new HumanName(){
                    Given = new List<string>() {"Test"},
                    Family = "Patient",
                    Use = HumanName.NameUse.Official
                }},
                BirthDateElement = new Date(DateTimeOffset.Now.Year - 20, DateTimeOffset.Now.Month,
                    DateTimeOffset.Now.Day)
            };

            //return Respond.WithResource(result);
            return new JsonResult(result);
        }

        [HttpGet("Practitioner/{id}")]
        [AllowAnonymous]
        public IActionResult GetPractitioner([FromRoute] string id)
        {
            Practitioner result = new Practitioner()
            {
                Id = id,
                Name = new List<HumanName>(){new HumanName(){
                    Given = new List<string>() {User.FindFirstValue("given_name")},
                    Family = User.FindFirstValue("family_name"),
                    Use = HumanName.NameUse.Official
                }},
                BirthDateElement = new Date(DateTimeOffset.Now.Year - 30, DateTimeOffset.Now.Month,
                    DateTimeOffset.Now.Day)
            };

            //return Respond.WithResource(result);
            return new JsonResult(result);
        }

        private string ConvertGenderToString(string gender)
        {
            string result;
            switch (gender)
            {
                case "M":
                    result = "male";
                    break;

                case "F":
                    result = "female";
                    break;

                case "O":
                    result = "other";
                    break;

                default:
                    result = "unknown";
                    break;
            }

            return result;
        }

        /// <summary>
        ///
        /// GET bundle.This will need to provide the following fields:
        /// Provider First Name
        ///    Provider Last Name
        /// Provider ID
        /// Provider ID Type
        /// Provider Role
        /// BC Personal Health Number
        /// Patient Last Name
        /// Patient First Name
        /// Date of Birth
        /// Gender
        /// Country
        /// Province/Territory
        /// State
        /// City/Town
        /// City/Town
        /// Address Use
        /// Driver Last Name
        /// Driver First Name
        /// Driver Date of Birth
        /// Driver Gender
        /// Country
        /// Province/Territory
        /// City/Town
        /// Street Address Line 1
        /// Street Address Line 2
        /// Postal Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Bundle/{id}")]
        [AllowAnonymous]
        public async Task<FhirResponse> GetBundle([FromRoute] string id)
        {
            Response.ContentType = "application/json";
            
            var getCaseRequest = new SearchRequest()
            {
                CaseId = id
            };
            var searchReply = _cmsAdapterClient.Search(getCaseRequest);

            var getCaseReply = searchReply.Items[0];

            Models.Driver driver;

            if (!String.IsNullOrEmpty(Configuration["ENABLE_ICBC"]))
            {
                // for test purposes the drivers licence number is stored in a configuration setting.

                string icbcDl = Configuration["TEST_DL"];
                CLNTRESPONSE icbcData = _icbcClient.GetDriver(icbcDl);
                driver = new Models.Driver()
                {
                    Surname = icbcData?.CLNT?.INAM?.SURN,
                    GivenName = icbcData?.CLNT?.INAM?.GIV1,
                    DriverLicenceNumber = icbcDl,
                    Address = new Models.Address()
                    {
                        City = icbcData?.CLNT?.ADDR?.CITY,
                        Postal = icbcData?.CLNT?.ADDR?.POST,
                        Line1 = $"{icbcData?.CLNT?.ADDR?.STNO} {icbcData?.CLNT?.ADDR?.STNM}",
                        Line2 = ""
                    }
                };
                if (icbcData?.CLNT?.BIDT != null)
                {
                    driver.BirthDate = icbcData.CLNT.BIDT.Value.ToString("yyyy-MM-dd");
                }
                if (icbcData?.CLNT?.SEX != null)
                {
                    driver.Sex = ConvertGenderToString(icbcData.CLNT?.SEX);
                }
            }
            else
            {
                // create from the Dynamics data
                driver = new Models.Driver()
                {
                    Surname = getCaseReply.Driver.Surname,
                    GivenName = getCaseReply.Driver.GivenName,
                    BirthDate = getCaseReply.Driver.BirthDate.ToDateTime().ToString("yyyy-MM-dd"),
                    DriverLicenceNumber = getCaseReply.Driver.DriverLicenceNumber,
                    Address = new Models.Address()
                    {
                        City = getCaseReply.Driver.Address.City,
                        Postal = getCaseReply.Driver.Address.Postal,
                        Line1 = getCaseReply.Driver.Address.Line1,
                        Line2 = getCaseReply.Driver.Address.Line2
                    }
                };

                if (!string.IsNullOrEmpty(getCaseReply.Driver.Sex))
                {
                    driver.Sex = getCaseReply.Driver.Sex.ToLower();
                }
                
            }
            
            Payload payload = new Payload
            {
                data = new Dictionary<string, object>
                {
                    //{"checkIsCommercialDMER", getCaseReply.Case.IsCommercial}, 
                    {"dropCommercialDMER", getCaseReply.IsCommercial ? "yes" : "no"},
                    {"providerNameGiven", "providerNameGiven"},
                    {"providerNameFamily", "providerNameFamily"},
                    {"providerId", "1234"},
                    {"providerIdType", "OPTID"},
                    {"providerRole", "Physician"},
                    {"providerSpecialty", "Cardiology"},
                    {"phoneUse", "work"},
                    {"providerPhoneNumber", "123-123-1234"}, // dashes are important here
                    {"providerPhoneNumberExt","123"},
                    {"faxUse", "work"},
                    {"providerFaxNumber", "123-123-1233"},
                    {"providerStreetAddressLine1", "providerStreetAddressLine1"},
                    {"providerStreetAddressLine2", "providerStreetAddressLine2"},
                    {"providerCityTown", "providerCityTown"},
                    {"patientIdentifier", Configuration["TEST_PHN"]},
                    {"patientNameFamily", $"{driver.Surname}"},
                    {"patientNameGiven", $"{driver.GivenName}"},
                    {"patientBirthDate", $"{driver.BirthDate}"},
                    {"gender", $"{driver.Sex}"}, // PHSA form expects lower case
                    {"patientCountry", "Canada"},
                    {"patientProvinceState", "British Columbia"},
                    {"patientCityTown", "Victoria"},
                    {"patientStreetAddressLine1", "patientStreetAddressLine1"},
                    {"patientStreetAddressLine2", "patientStreetAddressLine2"},
                    {"patientAddressPostalCode", "V1V 2V2"},
                    {"patientAddressUse", "patientAddressUse"},
                    {"patientNameGivenMiddle", "patientNameGivenMiddle"},
                    {"patientPrimaryPhoneNumber", "123-123-1234"},
                    {"patientPrimaryPhoneUse", "patientPrimaryPhoneUse"},
                    {"patientAlternatePhoneNumber", "patientAlternatePhoneNumber"},
                    {"patientAlternatePhoneUse", "patientAlternatePhoneUse"},
                    {"patientPrimaryEmail", "patientPrimaryEmail"},
                    {"patientPrimaryEmailUse", "home"},
                    {"patientAlternateEmail", "patientAlternateEmail"},
                    {"patientAlternateEmailUse", "work"},
                    {"textTargetDriverName", $"{driver.Surname}"},
                    {"textTargetDriverFirstname", $"{driver.GivenName}"},
                    {"textTargetDriverLicense", $"{driver.DriverLicenceNumber}"},
                    {"radioTargetDriverGender",$"{driver.Sex}"}, 
                    {"tDateTargetDriverBirthdate", $"{driver.BirthDate}"},
                    {"selTargetDriverCountry", "Canada"},
                    {"textTargetDriverProvince", "British Columbia"},
                    {"textTargetDriverCity", driver.Address.City},
                    {"textTargetDriverAddr1", $"{driver.Address.Line1}"}, 
                    {"textTargetDriverAddr2", $"{driver.Address.Line2}"},
                    {"textTargetDriverPostal", $"{driver.Address.Postal}"},
                    {"textTargetKnownNotice", ""},
                    {"patientProvince", "BC"},
                    {"patientBCcity", $"{driver.Address.City}"}
                }
            };

            // convert the data to json.
            string jsonPayload = JsonConvert.SerializeObject(payload);
            var jsonAsBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

            Bundle result = new Bundle()
            {
                Id = id,
                Meta = new Meta()
                {
                    LastUpdated = DateTimeOffset.Now,
                    Tag = new List<Coding>()
                    {
                        new Coding() { System = "https://ehealthbc.ca/NamingSystem/eforms/correlationId",
                            Code = "de487905-b0e1-49ed-911b-6a8de0806544"},
                        new Coding()
                        {
                            System="https://ehealthbc.ca/NamingSystem/eforms/formName",
                            Code = "DMFT-FULL-DMER-Container"
                        },
                        new Coding()
                        {
                            System = "https://ehealthbc.ca/NamingSystem/eforms/formTitle",
                            Code = "Development Testing Structured Form"
                        },
                        new Coding()
                        {
                            System="https://ehealthbc.ca/NamingSystem/eforms/formID",
                            Code = "609eb617894b2ab618b917ac"
                        },
                        new Coding()
                        {
                            System="https://ehealthbc.ca/NamingSystem/eforms/referenceNum",
                            Code = id
                        },
                        new Coding()
                        {
                            System="https://ehealthbc.ca/NamingSystem/eforms/formStatus",
                            Code ="InProgress"
                        }
                    }
                },
                Entry = new List<Bundle.EntryComponent>()
            {
                new Bundle.EntryComponent()
                {
                    Resource = new Binary()
                    {
                        ContentType = "application/eforms",
                        Data = jsonAsBytes
                    }
                },
               
                new Bundle.EntryComponent()
                {
                    Resource = new QuestionnaireResponse()
                    {
                        Id = id,
                        Item = new List<QuestionnaireResponse.ItemComponent>()
                        {
                            new QuestionnaireResponse.ItemComponent()
                            {
                                // "linkId":"dropCommercialDMER","text":"Is this a Commercial DMER?","answer":[{"valueString":"no"}
                                LinkId = "dropCommercialDMER",
                                Text = "Is this a Commercial DMER?",
                                Answer = new List<QuestionnaireResponse.AnswerComponent>()
                                {
                                    new QuestionnaireResponse.AnswerComponent()
                                    {
                                        Value = new FhirString(getCaseReply.IsCommercial ? "yes" : "no")
                                    }
                                }
                            }
                        }
                    }
                },
                new Bundle.EntryComponent()
                {
                    Resource = new Patient()
                    {
                        Id = "Patient1",
                        Gender = AdministrativeGender.Unknown,
                        Identifier = new List<Identifier>()
                        {
                            new Identifier()
                            {
                                Type = new CodeableConcept()
                                {
                                    Text = "BC"
                                },
                                System = Configuration["TEST_PHN_SYSTEM"],
                                Value = Configuration["TEST_PHN"]
                            }
                        },
                        Name = new List<HumanName>()
                        {
                            new HumanName()
                            {
                                Use = HumanName.NameUse.Official,
                                FamilyElement = new FhirString(driver.Surname),
                                GivenElement = new List<FhirString>()
                                    {new FhirString(driver.GivenName)}
                            }
                        },
                        Address = new List<Hl7.Fhir.Model.Address>()
                        {
                            new Hl7.Fhir.Model.Address()
                            {
                                Use = Hl7.Fhir.Model.Address.AddressUse.Home,
                                Line = new List<string>() {$"{driver.Address.Line1} {driver.Address.Line2}"},
                                State = "British Columbia",
                                Country = "Canada",
                                PostalCode = driver.Address.Postal
                            }
                        }
                    }
                },
                new Bundle.EntryComponent()
                {
                    Resource = new Practitioner()
                    {
                        Id = "Submitter1",
                        Identifier = new List<Identifier>()
                        {
                            new Identifier()
                            {
                                Type = new CodeableConcept()
                                {
                                    Text = "BC"
                                },
                                System = "[id-system-local-base]/ca-bc-provider-dummy-id",
                                Value = new Random().Next().ToString()
                            }
                        },
                        Gender = AdministrativeGender.Unknown,
                        Name = new List<HumanName>()
                        {
                            new HumanName()
                            {
                                Use = HumanName.NameUse.Official,
                                FamilyElement = new FhirString("DrSurname"),
                                GivenElement = new List<FhirString>()
                                    {new FhirString("DrGiven"), new FhirString("DrMiddle")}
                            }
                        },
                        Address = new List<Hl7.Fhir.Model.Address>()
                        {
                            new Hl7.Fhir.Model.Address()
                            {
                                Use = Hl7.Fhir.Model.Address.AddressUse.Home,
                                Line = new List<string>() {"123 Main St."},
                                State = "British Columbia",
                                Country = "Canada",
                                PostalCode = "V2V2V2"
                            }
                        }
                    }
                }
            }
            };

            return Respond.WithResource(result);
        }

        [HttpPut("Bundle/{id}")]
        [AllowAnonymous]
        public void PutBundle([FromBody] Bundle bundle, [FromRoute] string id)
        {
            FhirJsonSerializer serializer = new FhirJsonSerializer();
            // do something with bundle or id.
            _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(bundle.Children));
        }

        // save draft functionality
        [HttpPost("Bundle")]
        [AllowAnonymous]
        public async Task<IActionResult> PostBundle()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string body = await reader.ReadToEndAsync();

                // FhirJsonParser can return a typed object.
                // There is also a more generic FhirJsonNode.Parse
                // The built in Json Deserializer does not seem to work

                var parser = new FhirJsonParser();
                var bundle = parser.Parse<Bundle>(body);

                string dataFileKey = "";
                string pdfFileKey = "";
                Int64 dataFileSize = 0;
                Int64 pdfFileSize = 0;

                _logger.LogInformation(bundle.ToJson());

                // first pass to get the files.
                foreach (var entry in bundle.Entry)
                {
                    // find the PDF entry
                    if (entry.Resource.ResourceType == ResourceType.Binary &&
                        ((Binary)entry.Resource).ContentType == "application/pdf")
                    {
                        var b = (Binary)entry.Resource;
                        UploadFileRequest pdfData = new UploadFileRequest()
                        {
                            ContentType = "application/pdf",
                            Data = ByteString.CopyFrom(b.Data),
                            EntityName = "incident",
                            FileName = $"DMER.pdf",
                            FolderName = bundle.Id,
                        };

                        var reply = _documentStorageAdapterClient.UploadFile(pdfData);
                        pdfFileKey = reply.FileName;
                        pdfFileSize = pdfData.Data.Length;
                    }

                    if (entry.Resource.ResourceType == ResourceType.Binary &&
                        ((Binary)entry.Resource).ContentType == "application/eforms")
                    {
                        var b = (Binary)entry.Resource;
                        UploadFileRequest jsonData = new UploadFileRequest()
                        {
                            ContentType = "application/json",
                            Data = ByteString.CopyFrom(b.Data),
                            EntityName = "incident",
                            FileName = "data.json",
                            FolderName = bundle.Id
                        };

                        var reply = _documentStorageAdapterClient.UploadFile(jsonData);
                        dataFileKey = reply.FileName;
                        dataFileSize = jsonData.Data.Length;
                    }
                }

                // second pass to handle the questionnaire response
                foreach (var entry in bundle.Entry)
                {
                    
                    if (entry.Resource.ResourceType == ResourceType.QuestionnaireResponse)
                    {
                        var questionnaireResponse = (QuestionnaireResponse) entry.Resource;
                        // only triage completed items.
                        if (questionnaireResponse.StatusElement.Value ==
                            QuestionnaireResponse.QuestionnaireResponseStatus.Completed)
                        {

                            // convert the questionnaire response into json.
                            TriageRequest triageRequest = new TriageRequest()
                            {
                                Processed = false,
                                TimeCreated = Timestamp.FromDateTime(DateTimeOffset.Now.UtcDateTime),
                                Id = bundle.Id,
                                PdfFileKey = pdfFileKey,
                                PdfFileSize = pdfFileSize,
                                DataFileKey = dataFileKey,
                                DataFileSize = dataFileSize
                            };

                            triageRequest.AddItems(questionnaireResponse.Item);

                            // unlike the others this file is saved into a "folder" that can be used for queueing.
                            // S3 does not use folders like a file system, it is simple a convention for the key.
                            string jsonString = JsonConvert.SerializeObject(triageRequest);
                            UploadFileRequest jsonData = new UploadFileRequest()
                            {
                                ContentType = "application/json",
                                Data = ByteString.CopyFromUtf8(jsonString),
                                EntityName = "dfp",
                                FileName = $"{bundle.Id}.json",
                                FolderName = "triage-request"
                            };



                            // save a copy in the S3.
                            _documentStorageAdapterClient.UploadFile(jsonData);

                            // and send to the triage service.
                            _documentTriageClient.Triage(triageRequest);
                        }
                    }
                }


            }

            return Ok();
        }

        [HttpGet("QuestionnaireResponse/{id}")]
        [AllowAnonymous]
        public Questionnaire GetQuestionnaire([FromRoute] string id)
        {
            Questionnaire result = new Questionnaire();
            // id would be userpref

            return result;
        }
    }
}