using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
    public class Request
    {
        public string OperatorId { get; set; }

        public string BankCode { get; set; }

        public string RequestId { get; set; }
    }
}
