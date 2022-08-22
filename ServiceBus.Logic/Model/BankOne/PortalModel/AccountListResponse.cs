using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.PortalModel
{
   public  class AccountListResponse :BaseResponse
    {
        public List<Account> Accounts { get; set; }
    }

    public class SingleAccountResponse : BaseResponse
    {
        public Account Account { get; set; }
    }
}
