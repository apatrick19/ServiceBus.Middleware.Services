using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Servicebus.SMS.Gateway.Models
{
    public class ResponseModel
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseObject { get; set; }
    }
}