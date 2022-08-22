using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class DeliveryOptionReponse
    {
        public bool IsSuccessful { get; set; }
        public string ResponseDescription { get; set; }

        public List<DeliveryOptions> DeliveryOptions { get; set; }
    }

    public class DeliveryOptions
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
