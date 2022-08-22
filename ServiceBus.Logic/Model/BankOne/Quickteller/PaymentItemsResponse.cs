using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.PaymentItemSpace
{
    public class PaymentItemsResponse
    {
        public string Code { get; set; }
        public string BillerId { get; set; }
        public double Amount { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public string StatusDetails { get; set; }
        public bool RequestStatus { get; set; }
        public string ResponseDescription { get; set; }
        public string ResponseStatus { get; set; }
    }

 

}
