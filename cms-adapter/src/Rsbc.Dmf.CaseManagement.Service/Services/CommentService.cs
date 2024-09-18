using Microsoft.Extensions.Logging;
using AutoMapper;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;
using System;
using Rsbc.Dmf.CaseManagement.Manager.Comment;

namespace Rsbc.Dmf.CaseManagement.Service
{        
    
    public class CommentService : CommentManager.CommentManagerBase
    {
        private readonly ICommentManager _commentManager;
        private readonly ILogger<CommentService> _logger;
        private readonly IMapper _mapper;

        public CommentService(ICommentManager commentManager, ILogger<CommentService> logger, IMapper mapper)
        {
            _commentManager = commentManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async override Task<GetCommentsReply> GetCommentOnDriver(DriverIdRequest request, ServerCallContext context)
        {
            var reply = new GetCommentsReply();
            try
            {
                var result = await _commentManager.GetCommentOnDriver(Guid.Parse(request.Id));

                foreach (var item in result)
                {
                    var driver = new Driver();
                    if (item.Driver != null)
                    {
                        driver.DriverLicenseNumber = item.Driver.DriverLicenseNumber;
                        driver.Surname = item.Driver.Surname;
                        driver.Id = item.Driver.Id;
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
                        CommentText = item.CommentText ?? string.Empty,
                        SignatureName = item.SignatureName ?? string.Empty,
                        Origin = item.Origin ?? string.Empty,
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
        /// Add Case Comment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<CreateStatusReply> AddCaseComment(LegacyComment request, ServerCallContext context)
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

            var newComment = new Comment
            {
                CaseId = caseIdString,
                CommentText = request.CommentText,
                CommentTypeCode = request.CommentTypeCode,
                SequenceNumber = (int)request.SequenceNumber,
                UserId = request.UserId,
                Driver = driver,
                CommentDate = commentDate,
                CommentId = request.CommentId
            };

            var result = await _commentManager.AddCaseComment(newComment);

            if (result.Success)
            {
                reply.ResultStatus = ResultStatus.Success;
                reply.Id = result.Id;
            }
            else
            {
                reply.ResultStatus = ResultStatus.Fail;
                reply.ErrorDetail = result.ErrorDetail ?? string.Empty;
            }



            return reply;
        }
    }

   
}
