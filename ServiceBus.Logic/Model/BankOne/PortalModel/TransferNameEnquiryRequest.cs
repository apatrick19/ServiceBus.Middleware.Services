using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.PortalModel
{
    public class TransferNameEnquiryRequest
    {
        public string SourceAccountNo { get; set; }
        public string DestinationAccountNo { get; set; }
        public string BankCode { get; set; }
    }
}
