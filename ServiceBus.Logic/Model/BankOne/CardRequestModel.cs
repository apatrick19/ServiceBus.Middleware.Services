using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class CardRequestModel
    {
        public string Token { get; set; }
        public string BIN { get; set; }
        public string AccountNumber { get; set; }
        public string RequestType { get; set; }
        public string DeliveryOption { get; set; }
        public string Identifier { get; set; }
        public string NameOnCard { get; set; }
        
    }

    public class CardRequestModelPortal: CardRequestModel
    {
        public string ProfileName { get; set; }
      
    }
}
