using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Settings
{
    public class ECRSResponseModel
    {
        public string SetID { get; set; }
        public string ReferenceNumber { get; set; }
        public string RSAPIN { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string RequestType { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
