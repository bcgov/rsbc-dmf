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
using System.Reflection.Metadata;
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

        /// <summary>
        /// Create Legacy Case Comment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> CreateLegacyCaseComment(LegacyComment request, ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber;
                driver.Surname = request.Driver.Surname;
            }

            var commentDate = request.CommentDate.ToDateTimeOffset();

            if (commentDate.Year < 1753)
            {
                commentDate = DateTimeOffset.Now;
            }

            string caseIdString = null;
            Guid caseId;

            if (Guid.TryParse(request.CaseId, out caseId))
            {
                caseIdString = caseId.ToString();
            }

            var newComment = new CaseManagement.LegacyComment()
            {
                CaseId = caseIdString,
                CommentText = request.CommentText,
                CommentTypeCode = request.CommentTypeCode,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId,            
                Driver = driver,
                CommentDate = commentDate 
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
                reply.ErrorDetail = result.ErrorDetail;
            }

            return reply;
        }

        /// <summary>
        /// Create Legacy Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> CreateLegacyCaseDocument(LegacyDocument request, ServerCallContext context)
        {
            var reply = new CreateStatusReply();

            CaseManagement.Driver driver = new CaseManagement.Driver();
            if (request.Driver != null)
            {
                driver.DriverLicenseNumber = request.Driver.DriverLicenseNumber ?? string.Empty;
                driver.Surname = request.Driver.Surname ?? string.Empty;
            }

            var newDocument = new CaseManagement.LegacyDocument()
            {
                BatchId = request.BatchId ?? string.Empty,
                CaseId = request.CaseId ?? string.Empty,  
                DocumentId = request.DocumentId ?? string.Empty,
                DocumentPages = (int) request.DocumentPages,
                DocumentTypeCode = request.DocumentTypeCode ?? string.Empty,
                DocumentType = request.DocumentType ?? string.Empty,
                BusinessArea = request.BusinessArea ?? string.Empty,
                DocumentUrl = request.DocumentUrl ?? string.Empty,
                FaxReceivedDate = request.FaxReceivedDate.ToDateTimeOffset(),
                // may need to add FileSize,
                ImportDate = request.ImportDate.ToDateTimeOffset(),
                ImportId = request.ImportId ?? string.Empty,
                OriginatingNumber = request.OriginatingNumber ?? string.Empty,
                ValidationMethod = request.ValidationMethod ?? string.Empty,
                ValidationPrevious = request.ValidationPrevious ?? string.Empty,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId ?? string.Empty,
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

        /// <summary>
        /// Delete Legacy Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> DeleteComment(CommentIdRequest request, ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply() { ResultStatus = ResultStatus.Fail };

            // fetch the document.
            try
            {
                var d = await _caseManager.GetComment(request.CommentId);
                if (d != null)
                {
                    if (await _caseManager.DeleteComment(request.CommentId))
                    {
                        reply.ResultStatus = ResultStatus.Success;
                    }
                }
                else
                {
                    reply.ErrorDetail = "Comment ID not found";
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }


            return reply;
        }

        /// <summary>
        /// Delete Legacy Case Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> DeleteLegacyCaseDocument(LegacyDocumentRequest request, ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply() { ResultStatus = ResultStatus.Fail };

            // fetch the document.
            try
            {
                var d = await _caseManager.GetLegacyDocument(request.DocumentId);
                if (d != null)
                {
                    if (await _caseManager.DeleteLegacyDocument(request.DocumentId))
                    {
                        reply.ResultStatus = ResultStatus.Success;
                    }
                }
                else
                {
                    reply.ErrorDetail = "Document ID not found";
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }


            return reply;
        }

        /// <summary>
        /// Get Case Comments
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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
                        SequenceNumber = (long)(item.SequenceNumber ?? -1),
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

        /// <summary>
        /// Get Case Documents
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber ?? string.Empty;
                        driver.Surname = item.Driver.Surname ?? string.Empty;
                    }
                    reply.Items.Add(new LegacyDocument
                    {
                        BatchId = item.BatchId ?? string.Empty,
                        DocumentPages = item.DocumentPages,
                        DocumentTypeCode = item.DocumentTypeCode ?? string.Empty,
                   
                        CaseId = item.CaseId ?? string.Empty,
                        FaxReceivedDate = Timestamp.FromDateTimeOffset(item.FaxReceivedDate),
                        ImportDate = Timestamp.FromDateTimeOffset(item.ImportDate),
                        ImportId = item.ImportId ?? string.Empty,
                        
                        OriginatingNumber = item.OriginatingNumber ?? string.Empty,
                          
                        DocumentId = item.DocumentId ?? string.Empty,
                        SequenceNumber = (long)(item.SequenceNumber ?? -1),
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
                Serilog.Log.Error(ex, $"GetCaseDocuments {request.CaseId} Error");
                reply.ErrorDetail = ex.Message;
                reply.ResultStatus = ResultStatus.Fail;
            }
            return reply;
        }


        public async override Task<GetCommentReply> GetComment(CommentIdRequest request, ServerCallContext context)
        {
            var reply = new GetCommentReply();
            try
            {
                var item = await _caseManager.GetComment(request.CommentId);

                if (item != null)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                    }
                    reply.Item = new LegacyComment
                    {
                        CaseId = item.CaseId ?? string.Empty,
                        CommentDate = Timestamp.FromDateTimeOffset(item.CommentDate),
                        CommentTypeCode = item.CommentTypeCode ?? string.Empty,
                        CommentId = item.CommentId ?? string.Empty,
                        SequenceNumber = (long)(item.SequenceNumber ?? 0),
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        CommentText = item.CommentText ?? string.Empty
                    };
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

        /// <summary>
        /// Get Driver Comments
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// 


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

        /// <summary>
        /// Get All Driver Comments
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetCommentsReply> GetAllDriverComments(DriverLicenseRequest request, ServerCallContext context)
        {
            var reply = new GetCommentsReply();
            try
            {
                var result = await _caseManager.GetDriverLegacyComments(request.DriverLicenseNumber, true);

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

        /// <summary>
        /// Get Driver Documents
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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
                        driver.Surname = item.Driver.Surname ?? string.Empty;
                    }
                    reply.Items.Add(new LegacyDocument
                    {
                        BatchId = item.BatchId,
                        BusinessArea = item.BusinessArea,
                        CaseId = item.CaseId ?? string.Empty,
                        DocumentPages = item.DocumentPages,
                        DocumentId = item.DocumentId,
                        DocumentType = item.DocumentType ?? string.Empty,
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

        /// <summary>
        /// Get Drivers
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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
                        driver.BirthDate = Timestamp.FromDateTime(item.BirthDate.ToUniversalTime());
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


        /// <summary>
        /// Get Driver
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetDriversReply> GetDriver(DriverLicenseRequest request, ServerCallContext context)
        {
            var reply = new GetDriversReply();
            try
            {
                var result = await _caseManager.GetDriver(request.DriverLicenseNumber);

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item != null && item.DriverLicenseNumber != null)
                    {
                        driver.DriverLicenseNumber = item.DriverLicenseNumber;
                        driver.Surname = item.Surname ?? string.Empty;
                        driver.Id = item.Id;
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


        /// <summary>
        /// Process Legacy Candidate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<LegacyCandidateReply> ProcessLegacyCandidate(LegacyCandidateRequest request, ServerCallContext context)
        {
            var reply = new LegacyCandidateReply();

            // start by checking to see if there is an existing case.

            try
            {
                var searchRequest = new LegacyCandidateSearchRequest()
                {
                    DriverLicenseNumber = request.LicenseNumber,
                    Surname = request.Surname ?? string.Empty
                };
                var searchResult = await _caseManager.LegacyCandidateSearch(searchRequest);

                if (searchResult != null && searchResult.Items.Count() > 0)
                {
                    // case exists.
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    DateTimeOffset? dto = null;
                    if (request.EffectiveDate != null)
                    {
                        dto = request.EffectiveDate.ToDateTimeOffset();
                    }

                    DateTimeOffset? birthdate = null;

                    if (request.BirthDate != null)
                    {
                        birthdate = request.BirthDate.ToDateTimeOffset();
                    }

                    // create the case.
                    await _caseManager.LegacyCandidateCreate(searchRequest, birthdate, dto);

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

        /// <summary>
        /// Resolve Case Status Updates
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> ResolveCaseStatusUpdates(EmptyRequest request, ServerCallContext context)
        {
            var reply = new ResultStatusReply();

            try
            {
                // call case manager
                await _caseManager.ResolveCaseStatusUpdates();
            }

            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
            
        }

        /// <summary>
        /// Get List OfLettersSentToBcMail
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetDocumentsReply> GetListOfLettersSentToBcMail(EmptyRequest request, ServerCallContext context)
        {
            var reply = new GetDocumentsReply();

            try
            {
                // call case manager
                var result = await _caseManager.GetListOfLettersSentToBcMail();
                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber ?? string.Empty;
                        driver.Surname = item.Driver.Surname ?? string.Empty;
                    }
                    reply.Items.Add(new LegacyDocument
                    {
                        BatchId = item.BatchId ?? string.Empty,
                        DocumentPages = item.DocumentPages,
                        DocumentTypeCode = item.DocumentTypeCode ?? string.Empty,

                        CaseId = item.CaseId ?? string.Empty,
                        FaxReceivedDate = Timestamp.FromDateTimeOffset(item.FaxReceivedDate),
                        ImportDate = Timestamp.FromDateTimeOffset(item.ImportDate),
                        ImportId = item.ImportId ?? string.Empty,

                        OriginatingNumber = item.OriginatingNumber ?? string.Empty,

                        DocumentId = item.DocumentId ?? string.Empty,
                        SequenceNumber = (long)(item.SequenceNumber ?? -1),
                        UserId = item.UserId ?? string.Empty,
                        Driver = driver,
                        DocumentUrl = item.DocumentUrl ?? string.Empty,
                        ValidationMethod = item.ValidationMethod ?? string.Empty,
                        ValidationPrevious = item.ValidationPrevious ?? string.Empty

                    });
                }
                reply.ResultStatus = ResultStatus.Success;
            }

            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> UpdateDocumentStatus(LegacyDocumentStatusRequest request, ServerCallContext context)
        {
            var reply = new ResultStatusReply();

            try
            {
                // call case manager
                await _caseManager.UpdateDocumentStatus(request.DocumentId, (int)request.Status);
            }

            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;

        }


        /// <summary>
        /// Resolve Birthdate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> UpdateBirthDate(UpdateDriverRequest request, ServerCallContext context)
        {
            
            var reply = new ResultStatusReply();

            var driverRequest = new CaseManagement.UpdateDriverRequest
            {
                BirthDate = request.BirthDate.ToDateTime(),
                DriverLicenseNumber = request.DriverLicenseNumber,
            };
            
            try
            {
                if (request.BirthDate != null)
                {
                    // call case manager
                    await _caseManager.UpdateBirthDate(driverRequest);
                   
                    reply.ResultStatus = ResultStatus.Success;
                   
                }

            }

            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;

        }

        /// <summary>
        /// Case Search
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update Case
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Set Case Practitioner Clinic
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get All Flags
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get Unsent Medical Updates
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<SearchReply> GetUnsentMedicalUpdates(EmptyRequest request, ServerCallContext context)
        {
            var data = await _caseManager.GetUnsentMedicalUpdates();
                
            var cases = data.Items.Cast<Rsbc.Dmf.CaseManagement.DmerCase>();

            SearchReply reply = new(); 

            reply.Items.Add(cases.Select(c =>
            {
                
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
                    IsCommercial = c.IsCommercial,
                    ClinicName = c.ClinicName ?? string.Empty,
                    Status = c.Status
                };                

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

        /// <summary>
        /// Mark Medical Updates Sent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Mark Medical Update Error when ICBC fails to update
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> MarkMedicalUpdateError(IcbcErrorRequest request, ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply();
            try
            {
                var icbcErrorRequest = new CaseManagement.IcbcErrorRequest()
                {
                    CaseId = request.CaseId,
                    ErrorMessage = request.ErrorMessage
                };

                await _caseManager.MarkMedicalUpdateError(icbcErrorRequest);
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }
            
            return reply;
        }


        /// <summary>
        /// Mark Medical Update Error when ICBC fails to update
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> UpdateCleanPassFlag(CleanPassRequest request, ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply();
            try
            {
                var cleanPassRequest = new CaseManagement.CleanPassRequest()
                {
                    CaseId = request.CaseId,
                    isCleanPass = true
                };

                await _caseManager.UpdateCleanPassFlag(cleanPassRequest);
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = ex.Message;
            }

            return reply;
        }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get Legacy Document
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<GetLegacyDocumentReply> GetLegacyDocument(LegacyDocumentRequest request, ServerCallContext context)
        {
            GetLegacyDocumentReply reply = new GetLegacyDocumentReply();

            // fetch the document.
            try
            {
                var d = await _caseManager.GetLegacyDocument(request.DocumentId);
                reply.Document = new LegacyDocument
                {
                    BatchId = d.BatchId ?? string.Empty,
                    CaseId = d.CaseId ?? string.Empty,
                    DocumentId = d.DocumentId ?? string.Empty,
                    DocumentPages = (int)d.DocumentPages,
                    DocumentTypeCode = d.DocumentTypeCode ?? string.Empty,
                    DocumentUrl = d.DocumentUrl ?? string.Empty,
                    FaxReceivedDate = Timestamp.FromDateTimeOffset(d.FaxReceivedDate),
                    // may need to add FileSize,
                    ImportDate = Timestamp.FromDateTimeOffset(d.ImportDate),
                    ImportId = d.ImportId ?? string.Empty,
                    OriginatingNumber = d.OriginatingNumber ?? string.Empty,
                    ValidationMethod = d.ValidationMethod ?? string.Empty,
                    ValidationPrevious = d.ValidationPrevious ?? string.Empty,
                    SequenceNumber = (int)(d.SequenceNumber ?? -1),
                    UserId = d.UserId ?? string.Empty,                    
                };
                reply.ResultStatus = ResultStatus.Success;
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }
            

            return reply;
        }

        /// <summary>
        /// Create Bring Forwards
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ResultStatusReply> CreateBringForward(BringForwardRequest request, ServerCallContext context)
        {
            ResultStatusReply reply = new ResultStatusReply();

            try
            {
                var bringForwardRequest = new CaseManagement.BringForwardRequest()
                {
                    CaseId = request.CaseId ?? string.Empty,
                    Assignee = request.Assignee ?? string.Empty,
                    Description = request.Description?? string.Empty,                
                    Subject = request.Subject ?? string.Empty,
                    Priority = (CaseManagement.BringForwardPriority?)BringForwardPriority.Normal
                };
                
                // call _caseManager...
               var result =  await _caseManager.CreateBringForward(bringForwardRequest);

                if(result != null && result.Success)
                {
                    reply.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    reply.ResultStatus = ResultStatus.Fail;
                }
            }
            catch (Exception e)
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = e.Message;
            }

            return reply;
        }
    }
}