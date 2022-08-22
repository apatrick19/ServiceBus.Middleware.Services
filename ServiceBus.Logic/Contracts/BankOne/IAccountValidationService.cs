using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts
{
    public interface IAccountValidationService
    {
        ResponseModel GetAccountByAccountNo(Account account);
        ResponseModel GetBankOneBanks();
        ResponseModel GetProducts();
        StatementBaseResponse GetStatement(TransactionRequest request);
        ResponseModel GetTransaction(TransactionHistoryBaseRequest request);     
        ResponseModel GetAccountByAccountNo(string AccountNo);        
        ResponseModel AccountEnquiry(string AccountNo);
        ResponseModel ValidateBVN(string BVN);
        ResponseModel GetAllAccountsByAccountNo(string AccountNo, string CustomerId);
        ResponseModel FreezeAccount(FreezeAccountRequest request);
        ResponseModel UnFreezeAccount(FreezeAccountRequest request);
        ResponseModel ActivatePND(FreezeStatus request);
        ResponseModel DeactivatePND(FreezeStatus request);
        ResponseModel CheckPNDStatus(FreezeStatus request);
        ResponseModel CheckFreeze(FreezeStatus request);
        ResponseModel CheckLienStatus(FreezeStatus request);
        ResponseModel PlaceLien(PlaceLienModel request);
        ResponseModel UnPlaceLien(FreezeAccountRequest request);
    }
}
