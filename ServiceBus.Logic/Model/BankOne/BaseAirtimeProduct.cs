using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace manny.ussd.logic.Model
{
    public class BaseAirtimeProduct
    {
        public string Name { get; set; }
        public string PaymentCode { get; set; }
        public double Amount { get; set; }
    }
}
