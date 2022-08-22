using ServiceBus.Core.Contracts.Generic.Interface;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
   public class CablePaymentRequest : EntityStatus,ICablePaymentRequest
    {
       public string AccountNo { get; set; }
       public string SmartCardNo { get; set; }
        public string Package { get; set; }
        public decimal Amount { get; set; }
        public string PIN { get; set; }
    }
}
