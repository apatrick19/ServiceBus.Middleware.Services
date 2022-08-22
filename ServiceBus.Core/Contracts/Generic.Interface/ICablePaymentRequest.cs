using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
   public  interface ICablePaymentRequest:IEntityStatus
    {
         string SmartCardNo { get; set; }
         string Package { get; set; }
         decimal Amount { get; set; }
         string PIN { get; set; }
    }
}
