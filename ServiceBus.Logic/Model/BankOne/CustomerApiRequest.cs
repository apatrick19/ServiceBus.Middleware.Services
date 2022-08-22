using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class CustomerApiRequest:BaseRequest
    {
        public string CustomerId { get; set; }
        public string PaymentCode { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
    }
}
