using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
    public class GetAccountBalanceResponse:Response
    {
        public string AccountName { get; set; }     
        public string AccountNumber { get; set; }        
        public decimal LedgerBalance { get; set; }
        public decimal AvailableBalance { get; set; }
   
    }
}
