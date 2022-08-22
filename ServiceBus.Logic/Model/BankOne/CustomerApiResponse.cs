using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class CustomerApiResponse
    {
        public string CustomerId { get; set; }
        public string PaymentCode { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string ResponseCode { get; set; }
        public string FullName { get; set; }
        public string Amount { get; set; }
        public string AmountType { get; set; }
        public string AmountTypeDescription { get; set; }
    }
}
