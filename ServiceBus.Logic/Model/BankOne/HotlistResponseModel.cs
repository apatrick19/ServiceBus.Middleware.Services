using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public  class HotlistResponseModel
    {
        public bool IsSuccessful { get; set; }

        public string ResponseMessage { get; set; }
        public string SerialNo { get; set; }
        public string TransactionReference { get; set; }
    }
}
