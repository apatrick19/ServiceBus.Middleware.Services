using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public  class ChangePinModel
    {
        public string username { get; set; }
        public string oldpin { get; set; }
        public string newpin { get; set; }
        public string token { get; set; }
    }
}
