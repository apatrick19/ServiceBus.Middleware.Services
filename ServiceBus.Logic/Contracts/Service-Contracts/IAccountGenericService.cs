using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts.Service_Contracts
{
   public  interface IAccountGenericService
    {
        GetAccountByAccountNoResponse GetAccountByAccountNo(GetAccountByAccountNoRequest request);

        GetAccountBalanceResponse GetBalance(GetAccountBalanceRequest request);

        AccountCreationResponse CreateAccount(AccountRequest request);

        BVNValidationResponse ValidateBVN(BVNValidationRequest request);

        AccountEnquiryResponse AccountEnquiry(GetAccountByAccountNoRequest request);
    }
}
