using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBus.Core.DataTransferObject; 

namespace ServiceBus.Logic.Integration.Strategy
{
    public  interface IAccountByAccountNoIntegrationRepository
    {
        GetAccountByAccountNoResponse GetAccountByAccountNo(GetAccountByAccountNoRequest request);
        AccountEnquiryResponse AccountEnquiry(GetAccountByAccountNoRequest request);
        GetAccountBalanceResponse GetBalance(GetAccountBalanceRequest request);
    }
}
