using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    interface IBillingCategory
    {
         string Description { get; set; }
         bool IsAirtime { get; set; }
         int BillerCategoryID { get; set; }
         string ID { get; set; }
         string Name { get; set; }
         bool Status { get; set; }
         string StatusDetails { get; set; }
         string RequestStatus { get; set; }
         string ResponseDescription { get; set; }
         string ResponseStatus { get; set; }
    }
}
