using ServiceBus.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Transactions:Entity
    {
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ApplicationRefNo { get; set; }
        public string TransactionType { get; set; }
        public string CoreBankingRefNo { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Status { get; set; }
    }
}
