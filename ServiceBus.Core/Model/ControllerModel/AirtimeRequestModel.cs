using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.ControllerModel
{
    public class AirtimeRequestModel
    {
        public string DebitingAccountNo { get; set; }
        public string ProviderCode { get; set; }
        public string RecipientMobileNo { get; set; }
        public string RecipientEmail { get; set; }
        public decimal Amount { get; set; }
        public string PIN { get; set; }
    }

    public class SelfAirtimeRequestModel
    {
        public string AccountNo { get; set; }
        public string ProviderCode { get; set; }       
        public decimal Amount { get; set; }
        public string PIN { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
    }
}
