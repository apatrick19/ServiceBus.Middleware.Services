using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Servicebus.SMS.Gateway.Models
{
    public class SMSResponse
    {
        public string code { get; set; }
        public string message_id { get; set; }
        public string message { get; set; }
        public decimal balance { get; set; }
        public string user { get; set; }
    }
}