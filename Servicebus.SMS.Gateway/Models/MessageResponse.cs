using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Servicebus.SMS.Gateway.Models
{
    public class MessageResponse
    {
        public Data data { get; set; }
       // public int 0 { get; set; }
}

    public class Data
    {
        public string status { get; set; }
        public string message { get; set; }
    }

  
}

