using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rsbc.Dmf.CaseManagement.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using AutoMapper;

namespace Rsbc.Dmf.CaseManagement.Tests.Integration
{
    public class CommentServiceTests : WebAppTestBase
    {
        private readonly CommentService _commentService;
        private readonly IConfiguration _configuration;

        public CommentServiceTests(ITestOutputHelper output) : base(output)
        {
            var logger = services.GetRequiredService<ILogger<CommentService>>();
            var commentManager = services.GetRequiredService<ICommentManager>();
            var mapper = services.GetRequiredService<IMapper>();
            _configuration = services.GetRequiredService<IConfiguration>();
            _commentService = new CommentService(commentManager, logger, mapper);
        }

        [Fact(Skip = RequiresDynamics)]
        public async Task GetDriverComments()
        {
            var request = new DriverIdRequest();
            request.Id = _configuration["DRIVER_WITH_COMMENTS"];
            var response = await _commentService.GetCommentOnDriver(request, null);

            Assert.NotNull(response);
        }
    }
}
