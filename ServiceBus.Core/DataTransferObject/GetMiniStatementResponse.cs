using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
    public class GetMiniStatementResponse:Response
    {
        public string AccountNumber { get; set; }
        public List<MiniHistory> MiniHistory { get; set; }
       
    }

    public class MiniHistory
    {       
        public bool IsReversed { get; set; }           
        public string UniqueIdentifier { get; set; }      
        public DateTime TransactionDate { get; set; }        
        public string ReferenceID { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }       
        public decimal Balance { get; set; }
        public string PostingType { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
       
    }
}
