using Rsbc.Dmf.CaseManagement.Manager.Comment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface ICommentManager
    {
        Task<IEnumerable<Comment>> GetCommentOnDriver(Guid driverId);

        Task<CreateStatusReply> AddCaseComment(Comment request);
    }
}
