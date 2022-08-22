using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
   public class Transfer:Entity
    {
        public string Kyc { get; set; }
        public string ChannelCode { get; set; }
        public string SessionId { get; set; }
        public string NameEnquiryReference { get; set; }
        public string SourceBankCode { get; set; }
        public string SourceAccountNo { get; set; }
        public string SourceAccountName { get; set; }
        public string SourceBVN { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string BeneficiaryAccountName { get; set; }
        public string BeneficiaryBVN { get; set; }
        public string DestinationBankCode { get; set; }
        public string Narration { get; set; }
        public string PaymentReference { get; set; }
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
