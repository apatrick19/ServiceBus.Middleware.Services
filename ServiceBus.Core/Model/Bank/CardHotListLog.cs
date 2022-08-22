using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
    public class CardHotListLog:EntityStatus
    {
        public string AccountNumber { get; set; }
        public string SerialNo { get; set; }
        public string HotlistReason { get; set; }
        public string Token { get; set; }
        public string Reference { get; set; }
    }
}
