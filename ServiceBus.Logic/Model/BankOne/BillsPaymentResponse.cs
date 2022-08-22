using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class BillsPaymentResponse
    {
        public bool IsSuccessful { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string UniqueIdentifier { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseCode { get; set; }
        public string Reference { get; set; }
        public string RequestStatus { get; set; }
        public string ResponseDescription { get; set; }
    }

  
}
