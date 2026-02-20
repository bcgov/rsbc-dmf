using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pssg.Interfaces.Models
{
    public class IcbcNotificationsFileResult
    {
        public List<IFormFile> NotificationFiles { get; set; }
        public IEnumerable<string> ServerRelativeUrl { get; set; }
    }
}
