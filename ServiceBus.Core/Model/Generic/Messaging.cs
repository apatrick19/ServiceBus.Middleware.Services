using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
   public  class Messaging:Entity
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }

        //0 Pending; 1 Sent; 2 Failed; 3 Cancelled
        public int Status { get; set; }
    }
}
