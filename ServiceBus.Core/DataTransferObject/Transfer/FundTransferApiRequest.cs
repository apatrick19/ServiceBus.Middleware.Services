using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class FundTransferApiRequest
    {
        public decimal Amount { get; set; }
        public string AppzoneAccount { get; set; }
        public string Token { get; set; }
        public string PayerAccountNumber { get; set; }
        public string Payer { get; set; }
        public string ReceiverBankCode { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string ReceiverAccountType { get; set; }
        public string ReceiverKYC { get; set; }
        public string ReceiverBVN { get; set; }
        public string TransactionReference { get; set; }
        public string Narration { get; set; }
        public string NIPSessionID { get; set; }
        public string PIN { get; set; }

        public bool IsBeneficiaryTransfer { get; set; }
    }
}
