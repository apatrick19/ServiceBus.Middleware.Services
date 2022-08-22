using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public class TransactionBaseResponse
    {       
        public bool IsSuccessful { get; set; }
        public string CustomerIDInString { get; set; }
        public string Message { get; set; }
        public string TransactionTrackingRef { get; set; }
        public string Page { get; set; }
        
        
    }
}
