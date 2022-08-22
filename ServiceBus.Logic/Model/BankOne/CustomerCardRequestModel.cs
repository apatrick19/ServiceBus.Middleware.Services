using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public  class CustomerCardRequestModel
    {
        public string Token { get; set; }
        public string AccountNo { get; set; }
        public string CustomerID { get; set; }      
    }
}
