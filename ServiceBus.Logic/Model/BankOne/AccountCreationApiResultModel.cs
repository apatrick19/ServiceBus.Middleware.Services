using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.AccountResult
{
   
    public partial class AccountCreationApiResultModel
    {
        public bool IsSuccessful { get; set; }
        public string CustomerIDInString { get; set; }
        public Message Message { get; set; }
        public string TransactionTrackingRef { get; set; }
        public string Page { get; set; }
    }

    public class Message
    {
        public string AccountNumber { get; set; }
        public string CustomerID { get; set; }
        public string FullName { get; set; }
        public string CreationMessage { get; set; }
    }
      
}
