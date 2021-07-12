using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Rsbc.Dmf.PhsaAdapter.ViewModels;
using static Hl7.Fhir.Model.CapabilityStatement;
using JsonSerializer = System.Text.Json.JsonSerializer;
/*
namespace Hl7.Fhir.Model
{
    public partial class Resource
    {
        public Resource()
        {
            // just here for JSON...
            int i = 1;
            i++;
        }
    }
}
*/
namespace Rsbc.Dmf.PhsaAdapter.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class FhirController : ControllerBase
    {
        private readonly ILogger<ReceiveController> _logger;
        private readonly IConfiguration Configuration;
        private readonly IStructureDefinitionSummaryProvider _provider = new PocoStructureDefinitionSummaryProvider();

        public FhirController(ILogger<ReceiveController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
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
            capabilities.Add( "context-ehr-patient");
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
                Address = new List<Address>()
                {
                    new Address()
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
                    Given = new List<string>() {"Test"},
                    Family = "Practitioner",
                    Use = HumanName.NameUse.Official
                }},
                BirthDateElement = new Date(DateTimeOffset.Now.Year - 30, DateTimeOffset.Now.Month,
                    DateTimeOffset.Now.Day)
            };

            //return Respond.WithResource(result);
            return new JsonResult(result);
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
        public FhirResponse GetBundle([FromRoute] string id)
        {
            Response.ContentType = "application/json";
            /*
             * Issues - meta is null,
             * Identifier needs to be added
             */

            Payload payload = new Payload
            {
                data = new Dictionary<string, object>
                {
                    {"checkIsCommercialDMER", true},
                    {"isCommercialDMER", true},
                    {"providerNameGiven", "providerNameGiven"},
                    {"providerNameFamily", "providerNameFamily"},
                    {"providerId", "1234"},
                    {"providerIdType", "OPTID"},
                    {"providerRole", "Physician"},
                    {"providerSpecialty", "Cardiology"},
                    {"phoneUse", "work"},
                    {"providerPhoneNumber", "1231231234"},
                    {"providerPhoneNumberExt","123"},
                    {"faxUse", "work"},
                    {"providerFaxNumber", "1231231233"},
                    {"providerStreetAddressLine1", "providerStreetAddressLine1"},
                    {"providerStreetAddressLine2", "providerStreetAddressLine2"},
                    {"providerCityTown", "providerCityTown"},
                    {"patientIdentifier", Configuration["TEST_PHN"]},
                    {"patientNameFamily", "Family"},
                    {"patientNameGiven", "Given"},
                    {"patientBirthDate", "1970-01-01"},
                    {"gender", "male"},
                    {"patientCountry", "Canada"},
                    {"patientProvinceState", "British Columbia"},
                    {"patientCityTown", "Victoria"},
                    {"patientStreetAddressLine1", "patientStreetAddressLine1"},
                    {"patientStreetAddressLine2", "patientStreetAddressLine2"},
                    {"patientAddressPostalCode", "V1V 2V2"},
                    {"patientAddressUse", "patientAddressUse"},
                    {"patientNameGivenMiddle", "patientNameGivenMiddle"},
                    {"patientPrimaryPhoneNumber", "1231231234"},
                    {"patientPrimaryPhoneUse", "patientPrimaryPhoneUse"},
                    {"patientAlternatePhoneNumber", "patientAlternatePhoneNumber"},
                    {"patientAlternatePhoneUse", "patientAlternatePhoneUse"},
                    {"patientPrimaryEmail", "patientPrimaryEmail"},
                    {"patientPrimaryEmailUse", "home"},
                    {"patientAlternateEmail", "patientAlternateEmail"},
                    {"patientAlternateEmailUse", "work"},
                    {"textTargetDriverName", "DriverLastName"},
                    {"textTargetDriverFirstname", "DriverFirstname"},
                    {"textTargetDriverLicense","5888888"},
                    {"radioTargetDriverGender","male"},
                    {"tDateTargetDriverBirthdate", "1999-01-01"},
                    {"selTargetDriverCountry", "Canada"},
                    {"textTargetDriverProvince", "British Columbia"},
                    {"textTargetDriverCity", "Victoria"},
                    {"textTargetDriverAddr1", "textTargetDriverAddr1"},
                    {"textTargetDriverAddr2", "textTargetDriverAddr2"},
                    {"textTargetDriverPostal", "V1V 1V1"},
                    {"textTargetKnownNotice", "textTargetKnownNotice"},
                    {"selectMISC_0_0", "selectMISC_0_0y"},
                    {"yornMISC_1_1", "yornMISC_1_1"},
                    {"yornMISC_2_1", "yornMISC_2_1"},
                    {"yornMISC_3_1", "yornMISC_3_1"},
                    {"patientProvince", "BC"},
                    {"patientBCcity", "patientBCcity"}
                }
            };

            // convert the data to json.
            string jsonPayload = JsonConvert.SerializeObject(payload);
            var jsonAsBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            

            Bundle result = new Bundle()
            {
                Id = Guid.NewGuid().ToString(),
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
                            Code = "276fcdf2-d5bc-4c74-b81d-e0a8e2c71732"
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
                                FamilyElement = new FhirString("Surname"),
                                GivenElement = new List<FhirString>()
                                    {new FhirString("Given"), new FhirString("Middle")}
                            }
                        },
                        Address = new List<Address>()
                        {
                            new Address()
                            {
                                Use = Address.AddressUse.Home,
                                Line = new List<string>() {"123 Main St."},
                                State = "British Columbia",
                                Country = "Canada",
                                PostalCode = "V1V1V1"
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
                        Address = new List<Address>()
                        {
                            new Address()
                            {
                                Use = Address.AddressUse.Home,
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

                
                _logger.LogInformation(bundle.ToJson());

                //_logger.LogInformation(body);


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