using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
   public  class LocalFundTransferRequest : Request
    {
        public decimal Amount { get; set; }       
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public string RetrievalReference { get; set; }
        public string Narration { get; set; }      
    }
}
