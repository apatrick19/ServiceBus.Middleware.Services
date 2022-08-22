using ServiceBus.Core.Contracts;
using ServiceBus.Core.ControllerModel;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Custom.Model;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Contract
{
   public interface IAccountService
    {
        ResponseModel ValidateBVN(string  BVN);
        ResponseModel RegisterExistingAccount(ExistingAccountModel account);
        ResponseModel CreateNewAccount(AccountRequest account);

        ResponseModel GetAccounts(string accountNo);
        ResponseModel GetAccountByAccountNo(string accountNo);
        ResponseModel AccountEnquiry(string accountNo);
        ResponseModel UpdateDevice(DeviceUpdate request);

        ResponseModel GetTransaction(TransactionHistoryBaseRequest request);
        StatementBaseResponse GetStatement(TransactionRequest request);
        ResponseModel FreezeAccount(FreezeAccountRequest request);
        ResponseModel UnFreezeAccount(FreezeAccountRequest request);
        ResponseModel CheckFreeze(FreezeStatus request);
        ResponseModel PlaceLien(PlaceLienModel request);
        ResponseModel UnPlaceLien(FreezeAccountRequest request);
        ResponseModel CheckLienStatus(FreezeStatus request);

        ResponseModel ActivatePND(FreezeStatus request);
        ResponseModel DeactivatePND(FreezeStatus request);
        ResponseModel CheckPNDStatus(FreezeStatus request);
        //  ResponseModel GetTransaction(TransactionHistoryBaseRequest request);
    }
}
