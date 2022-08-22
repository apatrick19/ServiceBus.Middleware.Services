using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
    class GetBalanceResponse:Response
    {
        public string AccountName { get; set; }        
        public string AccountNumber { get; set; }     
        public double LedgerBalance { get; set; }
        public double AvailableBalance { get; set; }
       
    }
}
