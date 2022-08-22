using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class CardRequest:EntityStatus
    {
        public string BIN { get; set; }
        public string AccountNumber { get; set; }
        public string RequestType { get; set; }
        public string DeliveryOption { get; set; }
        public string Identifier { get; set; }
        public string NameOnCard { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
