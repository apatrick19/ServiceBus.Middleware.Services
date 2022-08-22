using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public class NameEnquiryRequest:Request
    {
        public string AccountNumber { get; set; }
        public string CommBankCode { get; set; }

    }



    public class NameEnquiryApiRequest 
    {
        public string AccountNumber { get; set; }
        public string Token { get; set; }
        public string BankCode { get; set; }
    }
}
