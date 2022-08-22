using ServiceBus.Core.Contracts.Bank.Interface;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
   public class AirtimeRequest: EntityStatus, IAirtimeRequest
    {
        public string DebitingAccountNo { get; set; }
        public string Biller { get; set; }
        public string RecipientMobileNo { get; set; }
        public decimal Amount { get; set; }
        public string PIN { get; set; }
    }
}
