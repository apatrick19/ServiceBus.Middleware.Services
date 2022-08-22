using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Email:EntityStatus,IEmail
    {
        public string Destination { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }       
        public string EmailId { get; set; }        
        public string RejectReason { get; set; }
    }
}
