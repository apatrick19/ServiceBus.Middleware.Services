using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class BankModel
    {
        public string Code { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public object StatusDetails { get; set; }
        public bool RequestStatus { get; set; }
        public object ResponseDescription { get; set; }
        public object ResponseStatus { get; set; }
    }

    public class ProductModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDiscriminator { get; set; }      
    }
}
