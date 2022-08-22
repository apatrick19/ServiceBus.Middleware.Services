using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class BaseRequest
    {
        public string CountryCode { get; set; }
        public string RequestId { get; set; }
        public string ReferenceNo { get; set; }
    }
}
