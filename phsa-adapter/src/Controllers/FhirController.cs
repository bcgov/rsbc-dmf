using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Rsbc.Dmf.PhsaAdapter.ViewModels;

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

namespace Rsbc.Dmf.PhsaAdapter.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class FhirController : ControllerBase
    {
        private readonly ILogger<ReceiveController> _logger;
        private readonly IConfiguration Configuration;

        public FhirController(ILogger<ReceiveController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        [HttpGet("metadata")]
        [AllowAnonymous]
        public CapabilityStatement GetMetaData()
        {
            CapabilityStatement result = new CapabilityStatement()
            {
                Date = DateTimeOffset.Now.ToString(),
                Kind = CapabilityStatementKind.Instance,
                FhirVersion = FHIRVersion.N4_0_0,
                Software = new CapabilityStatement.SoftwareComponent() {Name = "RSBC PHSA Adapter"},
                Status = PublicationStatus.Active,
                Format = new List<string>() {"application/fhir+json"},
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
            return result;
        }

        [HttpGet(".well-known/smart-configuration")]
        [AllowAnonymous]
        public SmartConfiguration GetSmartConfiguration()
        {
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
            return result;
        }

        [HttpGet("Patient/{id}")]
        [AllowAnonymous]
        public Patient GetPatient([FromRoute] string id)
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
            return result;
        }

        [HttpGet("Practitioner/{id}")]
        [AllowAnonymous]
        public Practitioner GetPractitioner([FromRoute] string id)
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
            return result;
        }

        [HttpGet("Bundle/{id}")]
        [AllowAnonymous]
        public Bundle GetBundle([FromRoute] string id)
        {

            Bundle result = new Bundle()
            {
                Id = Guid.NewGuid().ToString(),

            };
            return result;
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
                _logger.LogInformation(body);

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