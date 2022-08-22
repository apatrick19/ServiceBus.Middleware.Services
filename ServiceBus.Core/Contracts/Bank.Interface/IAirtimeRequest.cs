using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Bank.Interface
{
    public interface IAirtimeRequest:IEntityStatus
    {
         string DebitingAccountNo { get; set; }
         string Biller { get; set; }
         string RecipientMobileNo { get; set; }
         decimal Amount { get; set; }
         string PIN { get; set; }
    }
}
