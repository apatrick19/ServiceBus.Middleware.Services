using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class NameEnquiryResponse:Response
    {
        public string Name { get; set; }
        public string BVN { get; set; }
        public string KYC { get; set; }
        public bool IsSuccessful { get; set; }
        public string SessionID { get; set; }        
        public string DefaultGateWay { get; set; }
        public string InstitutionCode { get; set; }     
        public string AccountNumber { get; set; }
    }
}
