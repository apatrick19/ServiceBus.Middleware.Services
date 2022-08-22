using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Servicebus.SMS.Gateway.Models
{
    public class SMSRequest
    {
        public string MobileNo { get; set; }
        public string Message { get; set; }
    }
}