using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rsbc.Dmf.CaseManagement.Service.DecisionItem.Types;
using static Rsbc.Dmf.CaseManagement.Service.FlagItem.Types;

namespace Rsbc.Dmf.CaseManagement.Service
{
    //[Authorize]
    public class CaseService : CaseManager.CaseManagerBase
    {
        private readonly ILogger<CaseService> _logger;
        private readonly ICaseManager _caseManager;
        private readonly IConfiguration _configuration;

        public CaseService(ILogger<CaseService> logger, ICaseManager caseManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _caseManager = caseManager;
        }

        public async override Task<CreateStatusReply> CreateLegacyCaseComment(LegacyComment request, ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber;
                driver.Surname = request.Driver.Surname;
            }

            var newComment = new CaseManagement.LegacyComment()
            {
                CaseId = request.CaseId,
                CommentText = request.CommentText,
                CommentTypeCode = request.CommentTypeCode,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId,            
                Driver = driver
            };

            var result = await _caseManager.CreateLegacyCaseComment(newComment);

            if (result.Success )
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }


        public async override Task<CreateStatusReply> CreateLegacyCaseDocument(LegacyDocument request, ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber;
                driver.Surname = request.Driver.Surname;
            }

            var newDocument = new CaseManagement.LegacyDocument()
            {
                BatchId = request.BatchId,
                CaseId = request.CaseId,  
                DocumentId = request.DocumentId,
                DocumentPages = (int) request.DocumentPages,
                DocumentTypeCode = request.DocumentTypeCode,
                DocumentUrl = request.DocumentUrl,
                FaxReceivedDate = request.FaxReceivedDate.ToDateTimeOffset(),
                // may need to add FileSize,
                ImportDate = request.ImportDate.ToDateTimeOffset(),
                ImportId = request.ImportId,
                OriginatingNumber = request.OriginatingNumber,
                ValidationMethod = request.ValidationMethod,
                ValidationPrevious = request.ValidationPrevious,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId,
                Driver = driver 
            };

            var result = await _caseManager.CreateLegacyCaseDocument(newDocument);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
            }

            return reply;
        }

        public async override Task<GetCommentsReply> GetCaseComments(CaseIdRequest request, ServerCallContext context)
        {
            var reply = new GetCommentsReply();
            try
            {
                var result = await _caseManager.GetCaseLegacyComments(request.CaseId, false);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }
                    reply.Items.Add(new LegacyComment
                    {
                        CaseId = item.CaseId ?? string.Empty,
                        CommentDate = Timestamp.FromDateTimeOffset(item.CommentDate),
                        CommentTypeCode = item.CommentTypeCode ?? string.Empty,
                        CommentId = item.CommentId ?? string.Empty,
                        SequenceNumber = (long)item.SequenceNumber,
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        CommentText = item.CommentText ?? string.Empty
                    });
                }
                reply.ResultStatus = ResultStatus.Success;
                
                

            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }
            return reply;
        }


        public async override Task<GetDocumentsReply> GetCaseDocuments(CaseIdRequest request, ServerCallContext context)
        {
            var reply = new GetDocumentsReply();
            try
            {
                var result = await _caseManager.GetCaseLegacyDocuments(request.CaseId);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }
                    reply.Items.Add(new LegacyDocument
                    {
                        BatchId = item.BatchId ?? string.Empty,
                        DocumentPages = 0,
                        DocumentTypeCode = item.DocumentTypeCode ?? string.Empty,
                   
                        CaseId = item.CaseId ?? string.Empty,
                        FaxReceivedDate = Timestamp.FromDateTimeOffset(item.FaxReceivedDate),
                        ImportDate = Timestamp.FromDateTimeOffset(item.ImportDate),
                        ImportId = item.ImportId ?? string.Empty,
                         
                        OriginatingNumber = item.OriginatingNumber ?? string.Empty,
                          
                        DocumentId = item.DocumentId ?? string.Empty,
                        SequenceNumber = (long)item.SequenceNumber,
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        DocumentUrl = item.DocumentUrl ?? string.Empty,
                        ValidationMethod = item.ValidationMethod ?? string.Empty,
                        ValidationPrevious = item.ValidationPrevious ?? string.Empty
                        
                    });
                }
                reply.ResultStatus = ResultStatus.Success;



            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }
            return reply;
        }


        public async override Task<GetCommentsReply> GetDriverComments(DriverLicenseRequest request, ServerCallContext context)
        {
            var reply = new GetCommentsReply();
            try
            {
                var result = await _caseManager.GetDriverLegacyComments(request.DriverLicenseNumber, false);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }
                    reply.Items.Add(new LegacyComment
                    {
                        CaseId = item.CaseId ?? string.Empty,
                        CommentDate = Timestamp.FromDateTimeOffset(item.CommentDate),
                        CommentTypeCode = item.CommentTypeCode ?? string.Empty,
                        CommentId = item.CommentId ?? string.Empty,
                        SequenceNumber = (long)item.SequenceNumber,
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        CommentText = item.CommentText ?? string.Empty
                    });
                }
                reply.ResultStatus = ResultStatus.Success;

            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }
            return reply;
        }


        public async override Task<GetDocumentsReply> GetDriverDocuments(DriverLicenseRequest request, ServerCallContext context)
        {
            var reply = new GetDocumentsReply();
            try
            {
                var result = await _caseManager.GetDriverLegacyDocuments(request.DriverLicenseNumber);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }
                    reply.Items.Add(new LegacyDocument
                    {
                        BatchId = item.BatchId,
                        CaseId = item.CaseId,
                        DocumentPages = item.DocumentPages,
                        DocumentId = item.DocumentId,
                        DocumentTypeCode = item.DocumentTypeCode ?? string.Empty,
                        DocumentUrl = item.DocumentUrl ?? string.Empty,
                        FaxReceivedDate = Timestamp.FromDateTimeOffset(item.FaxReceivedDate),
                        ImportDate = Timestamp.FromDateTimeOffset(item.ImportDate),
                        ImportId = item.ImportId ?? string.Empty,
                        OriginatingNumber = item.OriginatingNumber ?? string.Empty,
                        ValidationMethod = item.ValidationMethod ?? string.Empty,
                        ValidationPrevious = item.ValidationPrevious ?? string.Empty,
                        SequenceNumber = item.SequenceNumber ?? -1,
                        Driver = driver
                        
                    });
                }
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }
            return reply;
        }


        public async override Task<GetDriversReply> GetDrivers(EmptyRequest request, ServerCallContext context)
        {
            var reply = new GetDriversReply();
            try
            {
                var result = await _caseManager.GetDrivers();

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item != null && item.DriverLicenseNumber != null)
                    {
                        driver.DriverLicenseNumber = item.DriverLicenseNumber;
                        driver.Surname = item.Surname ?? string.Empty;
                    }
                    reply.Items.Add(driver);
                }
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }
            return reply;
        }



        public async override Task<LegacyCandidateReply> ProcessLegacyCandidate(LegacyCandidateRequest request, ServerCallContext context)
        {
            var reply = new LegacyCandidateReply();

            // start by checking to see if there is an existing case.

            try
            {
                var searchRequest = new LegacyCandidateSearchRequest()
                {
                    DriverLicenseNumber = request.LicenseNumber,
                    Surname = request.Surname
                };
                var searchResult = await _caseManager.LegacyCandidateSearch(searchRequest);

                if (searchResult != null && searchResult.Items.Count() > 0)
                {
                    // case exists.
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    // create the case.
                    await _caseManager.LegacyCandidateCreate(searchRequest);
                    reply.ResultStatus = ResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }
                      
            return reply;
        }


        public async override Task<SearchReply> Search(SearchRequest request, ServerCallContext context)
        {
            var reply = new SearchReply();

            try
            {
                var cases = (await _caseManager.CaseSearch(new CaseSearchRequest
                {
                    CaseId = request.CaseId,
                    Title = request.Title,
                    ClinicId = request.ClinicId,
                    DriverLicenseNumber = request.DriverLicenseNumber
                })).Items.Cast<Rsbc.Dmf.CaseManagement.DmerCase>();

                reply.Items.Add(cases.Select(c =>
                {
                    Provider provider = null;
                    if (c.Provider != null)
                    {
                        provider = new Provider()
                        {
                            Id = c.Provider.Id,
                            Address = new Address()
                            {
                                City = c.Provider.Address.City ?? string.Empty,
                                Postal = c.Provider.Address.Postal ?? string.Empty,
                                Line1 = c.Provider.Address.Line1 ?? string.Empty,
                                Line2 = c.Provider.Address.Line2 ?? string.Empty,
                            },
                            FaxNumber = c.Provider.FaxNumber ?? string.Empty,
                            FaxUseType = c.Provider.FaxUseType ?? string.Empty,
                            GivenName = c.Provider.GivenName ?? string.Empty,
                            Surname = c.Provider.Surname ?? string.Empty,
                            Name = c.Provider.Name ?? string.Empty,
                            PhoneExtension = c.Provider.PhoneExtension ?? string.Empty,
                            PhoneNumber = c.Provider.PhoneNumber ?? string.Empty,
                            PhoneUseType = c.Provider.PhoneUseType ?? string.Empty,
                            ProviderDisplayId = c.Provider.ProviderDisplayId ?? string.Empty,
                            ProviderDisplayIdType = c.Provider.ProviderDisplayIdType ?? string.Empty,
                            ProviderRole = c.Provider.ProviderRole ?? string.Empty,
                            ProviderSpecialty = c.Provider.ProviderSpecialty ?? string.Empty
                        };
                    }

                    Driver driver = null;
                    if (c.Driver != null && c.Driver.Id != null) // only create a driver if it is a valid record
                    {
                        driver = new Driver {
                            Id = c.Driver.Id,
                            Surname = c.Driver.Surname ?? string.Empty,
                            Middlename = c.Driver.Middlename ?? string.Empty,
                            GivenName = c.Driver.GivenName ?? string.Empty,
                            BirthDate = Timestamp.FromDateTime(c.Driver.BirthDate.ToUniversalTime()),
                            DriverLicenseNumber = c.Driver.DriverLicenseNumber ?? string.Empty,
                            Address = new Address()
                            {
                                City = c.Driver.Address.City ?? string.Empty,
                                Postal = c.Driver.Address.Postal ?? string.Empty,
                                Line1 = c.Driver.Address.Line1 ?? string.Empty,
                                Line2 = c.Driver.Address.Line2 ?? string.Empty,
                            },
                            Sex = c.Driver.Sex ?? string.Empty,
                            Name = c.Driver.Name ?? string.Empty
                        };
                    }

                    var newCase = new DmerCase
                    {
                        CaseId = c.Id,
                        Title = c.Title ?? string.Empty,
                        CreatedBy = c.CreatedBy ?? string.Empty,
                        CreatedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                        ModifiedBy = c.CreatedBy ?? string.Empty,
                        ModifiedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                        Driver = driver,
                        Provider = provider,
                        IsCommercial = c.IsCommercial,
                        ClinicName = c.ClinicName ?? string.Empty,
                        Status = c.Status,
                        DmerType = c.DmerType ?? string.Empty,
                    };
                    newCase.Flags.Add(c.Flags.Select(f => new FlagItem
                    {
                        Identifier = f.Id, Question = f.Description ?? "Unknown", FlagType = ConvertFlagType(f.FlagType)
                    }));
                    return newCase;
                }));
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting case {request.CaseId}.");
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }


        public async override Task<UpdateCaseReply> UpdateCase(UpdateCaseRequest request, ServerCallContext context)
        {
            var reply = new UpdateCaseReply();
            try
            {
                _logger.LogInformation(
                    $"UPDATE CASE - {request.CaseId}, clean pass is {request.IsCleanPass}, files - {request.DataFileKey} {request.PdfFileKey}");

                // convert the flags to a list of strings.

                List<Flag> flags = new List<Flag>();

                foreach (var item in request.Flags)
                {
                    Flag newFlag = new Flag()
                    {
                        Description = item.Question,
                        Id = item.Identifier
                    };
                    flags.Add(newFlag);
                    _logger.LogInformation($"Added flag {item.Question} to flags for set case flags.");
                }

                // set the flags.

                var x = await _caseManager.SetCaseFlags(request.CaseId, request.IsCleanPass, flags);
                _logger.LogInformation($"Set Flags result is {x.Success}.");

                // update files.

                _logger.LogInformation(
                    $"Add file - {request.CaseId}, files - {request.DataFileKey} {request.PdfFileKey}");

                if (!string.IsNullOrEmpty(request.PdfFileKey))
                {
                    await _caseManager.AddDocumentUrlToCaseIfNotExist(request.CaseId, request.PdfFileKey, request.PdfFileSize);
                }

                if (!string.IsNullOrEmpty(request.DataFileKey))
                {
                    await _caseManager.AddDocumentUrlToCaseIfNotExist(request.CaseId, request.DataFileKey, request.DataFileSize);
                }
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while updating case.");
                reply.ErrorDetail = e.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }
            
            return reply;
        }

        FlagTypeOptions ConvertFlagType(FlagTypeOptionSet? value)
        {
            FlagTypeOptions result = FlagTypeOptions.Unknown;
            switch (value)
            {
                case FlagTypeOptionSet.FollowUp:
                    result = FlagTypeOptions.FollowUp;
                    break;
                case FlagTypeOptionSet.Message:
                    result = FlagTypeOptions.Message;
                    break;
                case FlagTypeOptionSet.Review:
                    result = FlagTypeOptions.Review;
                    break;
                case FlagTypeOptionSet.Submittal:
                    result = FlagTypeOptions.Submittal;
                    break;
            }

            return result;
        }

        public async override Task<SetCasePractitionerClinicReply> SetCasePractitionerClinic(SetCasePractitionerClinicRequest request, ServerCallContext context)
        {
            var reply = new SetCasePractitionerClinicReply();

            // call the case manager to update data
            try
            {
                await _caseManager.SetCasePractitionerClinic(request.ClinicId, request.PractitionerId, request.ClinicId);
            }
            catch (Exception ex)
            {
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }            

            return reply;

        }


            DecisionOutcomeOptions ConvertDecisionOutcome(DecisionOutcome? value)
        {
            DecisionOutcomeOptions result = DecisionOutcomeOptions.Unknown;
            switch (value)
            {
                case DecisionOutcome.NonComply:
                    result = DecisionOutcomeOptions.NonComply;
                    break;
                case DecisionOutcome.FitToDrive:
                    result = DecisionOutcomeOptions.FitToDrive;
                    break;
                case DecisionOutcome.UnfitToDrive:
                    result = DecisionOutcomeOptions.UnfitToDrive;
                    break;                
            }

            return result;
        }


        public async override Task<GetAllFlagsReply> GetAllFlags(EmptyRequest request, ServerCallContext context)
        {
            var reply = new GetAllFlagsReply();
            var flags = await _caseManager.GetAllFlags();
            foreach (var flag in flags)
            {
                FlagItem newFlag = new FlagItem()
                {
                    Identifier = flag.Id,
                    Question = flag.Description ?? "",
                    FlagType = ConvertFlagType (flag.FlagType)
                };
                reply.Flags.Add(newFlag);
            }

            return reply;
        }


        public async override Task<SearchReply> GetUnsentMedicalUpdates(EmptyRequest request, ServerCallContext context)
        {
            var data = await _caseManager.GetUnsentMedicalUpdates();
                
            var cases = data.Items.Cast<Rsbc.Dmf.CaseManagement.DmerCase>();

            SearchReply reply = new SearchReply(); 

            reply.Items.Add(cases.Select(c =>
            {
                Provider provider = null;
                if (c.Provider != null)
                {
                    provider = new Provider()
                    {
                        Id = c.Provider.Id,
                        Address = new Address()
                        {
                            City = c.Provider.Address.City ?? string.Empty,
                            Postal = c.Provider.Address.Postal ?? string.Empty,
                            Line1 = c.Provider.Address.Line1 ?? string.Empty,
                            Line2 = c.Provider.Address.Line2 ?? string.Empty,
                        },
                        FaxNumber = c.Provider.FaxNumber ?? string.Empty,
                        FaxUseType = c.Provider.FaxUseType ?? string.Empty,
                        GivenName = c.Provider.GivenName ?? string.Empty,
                        Surname = c.Provider.Surname ?? string.Empty,
                        Name = c.Provider.Name ?? string.Empty,
                        PhoneExtension = c.Provider.PhoneExtension ?? string.Empty,
                        PhoneNumber = c.Provider.PhoneNumber ?? string.Empty,
                        PhoneUseType = c.Provider.PhoneUseType ?? string.Empty,
                        ProviderDisplayId = c.Provider.ProviderDisplayId ?? string.Empty,
                        ProviderDisplayIdType = c.Provider.ProviderDisplayIdType ?? string.Empty,
                        ProviderRole = c.Provider.ProviderRole ?? string.Empty,
                        ProviderSpecialty = c.Provider.ProviderSpecialty ?? string.Empty
                    };
                }

                Driver driver = null;
                if (c.Driver != null && c.Driver.Id != null)
                {
                    driver = new Driver()
                    {
                        Id = c.Driver.Id ?? string.Empty,
                        Surname = c.Driver.Surname ?? string.Empty,
                        GivenName = c.Driver.GivenName ?? string.Empty,
                        BirthDate = Timestamp.FromDateTime(c.Driver.BirthDate.ToUniversalTime()),
                        DriverLicenseNumber = c.Driver.DriverLicenseNumber ?? string.Empty,
                        Address = new Address()
                        {
                            City = c.Driver.Address.City ?? string.Empty,
                            Postal = c.Driver.Address.Postal ?? string.Empty,
                            Line1 = c.Driver.Address.Line1 ?? string.Empty,
                            Line2 = c.Driver.Address.Line2 ?? string.Empty,
                        },
                        Sex = c.Driver.Sex ?? string.Empty,
                        Name = c.Driver.Name ?? string.Empty
                    };
                }
                var newCase = new DmerCase
                {
                    CaseId = c.Id,
                    Title = c.Title ?? string.Empty,
                    CreatedBy = c.CreatedBy ?? string.Empty,
                    CreatedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                    ModifiedBy = c.CreatedBy ?? string.Empty,
                    ModifiedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                    Driver = driver,
                    Provider = provider,
                    IsCommercial = c.IsCommercial,
                    ClinicName = c.ClinicName ?? string.Empty,
                    Status = c.Status
                };
                newCase.Flags.Add(c.Flags.Select(f => new FlagItem
                {
                    Identifier = f.Id,
                    Question = f.Description ?? "Unknown",
                    FlagType = ConvertFlagType(f.FlagType)
                }));

                newCase.Decisions.Add(c.Decisions.Select(d => new DecisionItem
                {
                    Identifier = d.Id,
                    Outcome = ConvertDecisionOutcome(d.Outcome),
                    CreatedOn = Timestamp.FromDateTime(d.CreatedOn.DateTime.ToUniversalTime())
                }));

                return newCase;
            }));

            return reply;
        }

        public async override Task<ResultStatusReply> MarkMedicalUpdatesSent(IdListRequest request, ServerCallContext context)
        {
            ResultStatusReply result = new ResultStatusReply();
            try
            {
                await _caseManager.MarkMedicalUpdatesSent(request.IdList.ToList());
                
                result.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = ex.Message;
            }
            
            return result;
        }

        

        [AllowAnonymous]
        public override Task<TokenReply> GetToken(TokenRequest request, ServerCallContext context)
        {
            var result = new TokenReply();
            result.ResultStatus = ResultStatus.Fail;

            var configuredSecret = _configuration["JWT_TOKEN_KEY"];
            if (!string.IsNullOrEmpty(request?.Secret) && configuredSecret.Equals(request.Secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["JWT_VALID_ISSUER"],
                    _configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddYears(5),
                    signingCredentials: creds
                );
                result.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                result.ResultStatus = ResultStatus.Success;
            }
            else
            {
                result.ErrorDetail = "Bad Request";
            }

            return Task.FromResult(result);
        }
    }
}