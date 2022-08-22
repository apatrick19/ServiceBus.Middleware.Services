using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class FundTransferResponse:Response
    {      
       public string Reference { get; set; }       
    }


    public class FundTransferApiResponse : Response
    {
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public int ReferenceID { get; set; }
        public string UniqueIdentifier { get; set; }
        public bool IsSuccessFul { get; set; }
        public string Reference { get; set; }
        public object SessionID { get; set; }
        public bool RequestStatus { get; set; }
        public string ResponseDescription { get; set; }
        public string ResponseStatus { get; set; }
    }
}
