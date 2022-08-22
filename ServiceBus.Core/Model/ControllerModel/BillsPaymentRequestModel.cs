using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.ControllerModel
{
    public class BillsPaymentRequestModel
    {
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string CustomerId { get; set; }
        public string ProductCode { get; set; }
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public string PIN { get; set; }
    }
}
