using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Logic.Model.PortalModel
{
    public class AccountReportResponse: BaseResponse
    {
        public List<Account> Accounts { get; set; }

    }


}