using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
   public  class Response
    {
        
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string RequestId { get; set; }
        public string OperatorId { get; set; }
        public string BankId { get; set; }
    }
}
