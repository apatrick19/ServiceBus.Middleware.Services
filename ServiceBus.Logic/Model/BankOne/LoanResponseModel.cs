using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.LoanSpace
{
   public  class LoanResponseModel
    {
        public bool IsSuccessful { get; set; }
        public string CustomerIDInString { get; set; }
        public Message Message { get; set; }
        public string TransactionTrackingRef { get; set; }
        public object Page { get; set; }
    }

    public class Message
    {
        public string AccountNumber { get; set; }
        public string CustomerID { get; set; }
        public string FullName { get; set; }
        public object CreationMessage { get; set; }
    }
}
