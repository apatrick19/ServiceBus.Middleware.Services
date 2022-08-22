using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public  class HotlistRequestModel
    {
        public string AccountNumber { get; set; }
        public string SerialNo { get; set; }
        public string HotlistReason { get; set; }
        public string Token { get; set; }
        public string Reference { get; set; }
    }
}
