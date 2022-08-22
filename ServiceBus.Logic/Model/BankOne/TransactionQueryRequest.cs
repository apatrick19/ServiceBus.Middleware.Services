using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public class TransactionQueryRequest:Request
    {
        public string RetrievalReference { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        
    }


    public class TransactionQueryApiRequest
    {
        public string RetrievalReference { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Token { get; set; }
    }

}
