using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
   public  class BillsPaymentTransaction:EntityStatus
    {
        public string SourceAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Beneficiary { get; set; }
        public string Narration { get; set; }
        public bool isSuccessful { get; set; }
        public string CoreBankingResponseCode { get; set; }
        public string CoreBankingResponseMessage { get; set; }
        public string ThirdPartyResponseCode { get; set; }
        public string ThirdPartyResponseMessage { get; set; }
    }
}
