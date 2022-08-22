using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
  public   class BankOneSMSRequest
    {
        public List<SMSModel> SMSModels { get; set; }
    }


    public class SMSModel
    {
        public string AccountNumber { get; set; }
        public string To { get; set; }
        public string AccountId { get; set; }
        public string Body { get; set; }
        public string ReferenceNo { get; set; }
    }

   
}
