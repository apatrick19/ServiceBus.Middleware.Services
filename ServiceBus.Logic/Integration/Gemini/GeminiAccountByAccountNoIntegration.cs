using ServiceBus.Core.DataTransferObject;
using ServiceBus.Logic.Contracts.BankOne;
using ServicBus.Logic.Contracts;
using ServiceBus.Core;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Settings;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Contracts.BankOne;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration.Gemini
{
    public  class GeminiAccountByAccountNoIntegration: IAccountByAccountNoIntegration
    {
        string className = "GeminiAccountByAccountNoIntegration";

        IApiPostAndGet apiservice;

        public GeminiAccountByAccountNoIntegration(IApiPostAndGet service)
        {

            apiservice = service;
        }

        public GetAccountByAccountNoResponse GetAccountByAccountNo(GetAccountByAccountNoRequest request)
        {
            string methodName = "GetAccountByAccountNo";
            try
            {
                var response = new GetAccountByAccountNoResponse();

                string Url = $"{BaseService.GetAppSetting("GeminiCoreBankingBaseUrl")}Account/GetAccountSummary/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={request.AccountNumber}&institutionCode={BaseService.GetAppSetting("BankOneCode")}";

                var acctResult = apiservice.UrlGet<BankOneAccountSummaryModel>(Url, "");
                if (acctResult == null)
                {
                    return new GetAccountByAccountNoResponse() { ResponseCode = "01", ResponseMessage = " no response from server", OperatorId = request.OperatorId, };
                }
                if (acctResult.IsSuccessful == false)
                {
                    return new GetAccountByAccountNoResponse() { ResponseCode = "04", ResponseMessage = " no record found / invalid account number", OperatorId = request.OperatorId, };
                }
                response.AvailableBalance = acctResult.Message.LedgerBalance / 100;
                response.LedgerBalance = acctResult.Message.LedgerBalance / 100;
                response.AccountName = acctResult.Message.Name;
                response.AccountNumber = acctResult.Message.NUBAN;
                response.AccountType = acctResult.Message.Product;
                response.BranchId = acctResult.Message.Branch;
                response.PhoneNumber = acctResult.Message.PhoneNumber;
                response.Status = acctResult.Message.AccountStatus;
                response.CustomerName = acctResult.Message.Name;
                response.NotificationPreference = acctResult.Message.NotificationPreference;
                response.StatementPreference = acctResult.Message.StatementPreference.Delivery;
                response.TransactionPermission = acctResult.Message.TransactionPermission;
                response.Email = acctResult.Message.Email;
                response.BranchId = acctResult.Message.Branch;
                response.NotificationPreference = acctResult.Message.NotificationPreference;
                response.ResponseCode = "00";
                response.ResponseMessage = "Request Successful";
                response.OperatorId = request.OperatorId;
                response.BankId = request.BankCode;
                response.RequestId = response.RequestId;

                return response;
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    GetAccountByAccountNoResponse()
                    { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }

        public AccountEnquiryResponse AccountEnquiry(GetAccountByAccountNoRequest request)
        {
            string methodName = "AccountEnquiry";
            try
            {
                var response = new AccountEnquiryResponse();

                var payload = new { AccountNo = request.AccountNumber, AuthenticationCode = BaseService.GetAppSetting("AuthToken") };
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/AccountEnquiry";
                var acctResult = apiservice.UrlPost<AccountEnquiryModel>(Url, payload);
                if (acctResult == null)
                {
                    return new AccountEnquiryResponse() { ResponseCode = "01", ResponseMessage = " no response from server", OperatorId = request.OperatorId, };
                }
                if (acctResult.IsSuccessful == false)
                {
                    return new AccountEnquiryResponse() { ResponseCode = "04", ResponseMessage = " no record found / invalid account number", OperatorId = request.OperatorId, };
                }

                response.AvailableBalance = acctResult.LedgerBalance / 100;
                response.LedgerBalance = acctResult.LedgerBalance / 100;
                response.Name = acctResult.Name;
                response.Nuban = acctResult.Nuban;
                response.Tier = acctResult.Tier;
                response.FirstName = acctResult.FirstName;
                response.LastName = acctResult.LastName;
                response.Status = acctResult.Status;
                response.MaximumBalance = acctResult.MaximumBalance;
                response.MaximumDeposit = acctResult.MaximumDeposit;
                response.LienStatus = acctResult.LienStatus;
                response.Number = acctResult.Number;
                response.Email = acctResult.Email;
                response.PhoneNuber = acctResult.PhoneNo;
                response.PNDStatus = acctResult.PNDStatus;

                response.ProductCode = acctResult.ProductCode;
                response.FreezeStatus = acctResult.FreezeStatus;
                response.BVN = acctResult.BVN;

                response.OperatorId = request.OperatorId;
                response.BankId = request.BankCode;
                response.RequestId = request.RequestId;

                response.ResponseCode = "00";
                response.ResponseMessage = "Request Successful";
                return response;
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    AccountEnquiryResponse()
                    { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }

        public GetAccountBalanceResponse GetBalance(GetAccountBalanceRequest request)
        {
            string methodName = "GetBalance";
            try
            {
                var response = new GetAccountBalanceResponse();

                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GetAccountSummary/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={request.AccountNumber}&institutionCode={BaseService.GetAppSetting("BankOneCode")}";

                var acctResult = apiservice.UrlGet<BankOneAccountSummaryModel>(Url, "");
                if (acctResult == null)
                {
                    return new GetAccountBalanceResponse() { ResponseCode = "01", ResponseMessage = " no response from server", OperatorId = request.OperatorId, };
                }
                if (acctResult.IsSuccessful == false)
                {
                    return new GetAccountBalanceResponse() { ResponseCode = "04", ResponseMessage = " no record found / invalid account number", OperatorId = request.OperatorId, };
                }
                response.AvailableBalance = acctResult.Message.LedgerBalance / 100;
                response.LedgerBalance = acctResult.Message.LedgerBalance / 100;
                response.AccountName = acctResult.Message.Name;
                response.AccountNumber = acctResult.Message.NUBAN;
                response.ResponseCode = "00";
                response.ResponseMessage = "Request Successful";
                response.OperatorId = request.OperatorId;
                response.BankId = request.BankCode;
                response.RequestId = response.RequestId;



                return response;
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    GetAccountBalanceResponse()
                    { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }
    }
}
