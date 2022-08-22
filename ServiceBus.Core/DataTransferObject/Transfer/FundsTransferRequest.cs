using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
    public class FundsTransferRequest:Request
    {
        public decimal Amount { get; set; }  
        public string PayerAccountNumber { get; set; }
        public string Payer { get; set; }
        public string ReceiverBankCode { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string ReceiverName { get; set; }
        public string TransactionReference { get; set; }
        public string Narration { get; set; }
        public string NIPSessionID { get; set; }
       
    }
}
