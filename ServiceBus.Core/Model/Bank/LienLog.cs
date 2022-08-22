using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
    public class LienLog:EntityStatus
    {
        public string AccountNo { get; set; }
        public string AuthenticationCode { get; set; }
        public string ReferenceID { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }

        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
