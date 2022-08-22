using Newtonsoft.Json;
using ServicBus.Logic.Contracts;
using ServicBus.Logic.Implementations;
using ServiceBus.Core;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Settings;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.AccountInfo;
using ServiceBus.Logic.Model.CorebankingList;
using ServiceBus.Logic.Model.History;
using ServiceBus.Logic.Model.Transactions;
using ServiceBus.Logic.Model.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class AccountValidationService : IAccountValidationService
    {
         string className = "AccountValidationService";
        IBankOneCoreBankingAuthentication coreBankingService;
        IApiPostAndGet apiservice;
        
        public AccountValidationService(IBankOneCoreBankingAuthentication corebanking, IApiPostAndGet service)
        {
            coreBankingService = corebanking;
            apiservice = service;
        }
        public ResponseModel GetAccountByAccountNo(string AccountNo)
        {
            try
            {
                var acctModel = new AccountDetailsModel();
              
                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GetAccountSummary/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={AccountNo}&institutionCode={BaseService.GetAppSetting("BankOneCode")}";
                var acctResult = apiservice.UrlGet<BankOneAccountSummaryModel>(Url,"");
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.IsSuccessful==false)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                acctModel.AvailableBalance = acctResult.Message.LedgerBalance;
                acctModel.LedgerBalance = acctResult.Message.LedgerBalance;
                acctModel.AccountName = acctResult.Message.Name;
                acctModel.AccountNumber = acctResult.Message.NUBAN;
                acctModel.AccountType = acctResult.Message.Product;
                acctModel.BranchId = acctResult.Message.Branch;
                acctModel.PhoneNumber = acctResult.Message.PhoneNumber;
                acctModel.Status = acctResult.Message.AccountStatus;
                acctModel.CustomerName = acctResult.Message.Name;
                acctModel.NotificationPreference = acctResult.Message.NotificationPreference;
                acctModel.StatementPreference = acctResult.Message.StatementPreference.Delivery;
                acctModel.TransactionPermission = acctResult.Message.TransactionPermission;
                acctModel.Email = acctResult.Message.Email;
                acctModel.BranchId = acctResult.Message.Branch;
                acctModel.NotificationPreference = acctResult.Message.NotificationPreference;
                return ResponseDictionary.GetCodeDescription("00", "Successful", acctModel);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }   
        }

        public ResponseModel GetAccountByAccountNo(Account account)
        {
            try
            {
                var acctModel = new AccountDetailsModel();

                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GetAccountSummary/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={account.AccountNumber}&institutionCode={BaseService.GetAppSetting("BankOneCode")}";
                var acctResult = apiservice.UrlGet<BankOneAccountSummaryModel>(Url, "");
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                acctModel.AvailableBalance = acctResult.Message.AvailableBalance;
                acctModel.LedgerBalance = acctResult.Message.LedgerBalance;
                acctModel.AccountName = acctResult.Message.Name;
                acctModel.AccountNumber = acctResult.Message.NUBAN;
                acctModel.AccountType = acctResult.Message.Product;
                acctModel.BranchId = acctResult.Message.Branch;
                acctModel.PhoneNumber = acctResult.Message.PhoneNumber;
                acctModel.Status = acctResult.Message.AccountStatus;
                acctModel.CustomerName = acctResult.Message.Name;
                acctModel.NotificationPreference = acctResult.Message.NotificationPreference;
                acctModel.StatementPreference = acctResult.Message.StatementPreference.Delivery;
                acctModel.TransactionPermission = acctResult.Message.TransactionPermission;
                acctModel.Email = acctResult.Message.Email;
                acctModel.BranchId = acctResult.Message.Branch;
                acctModel.NotificationPreference = acctResult.Message.NotificationPreference;


                account.AccountName = acctResult.Message.Name;
                account.AccountNumber = acctResult.Message.NUBAN;
                account.ProductCode = acctResult.Message.Product;
                account.MobileNo = acctResult.Message.PhoneNumber;
                account.Status = acctResult.Message.AccountStatus;
                account.Email = acctResult.Message.Email;
                try
                {
                    account.FirstName = acctResult.Message.Name.Split(' ')[0];
                    account.LastName = acctResult.Message.Name.Split(' ')[1];
                }
                catch (Exception)
                {

                }                 
                return ResponseDictionary.GetCodeDescription("00", "Successful", acctModel);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static BankOneAccountSummaryModel GetAccountByAccountNoStatic(Account account)
        {
            try
            {
               
                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GetAccountSummary/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={account.AccountNumber}&institutionCode={BaseService.GetAppSetting("BankOneCode")}";
                var acctResult = new ApiPostAndGet().UrlGet<BankOneAccountSummaryModel>(Url, "");
                if (acctResult == null)
                {
                    return new BankOneAccountSummaryModel() { ResponseCode = "04", ResponseMessage = "no record found" };
                   
                }
                if (acctResult.IsSuccessful == false)
                {
                    return new BankOneAccountSummaryModel() { ResponseCode = "04", ResponseMessage = "no record found" };
                }
              

                account.AccountName = acctResult.Message.Name;
                account.AccountNumber = acctResult.Message.NUBAN;
                account.ProductCode = acctResult.Message.Product;
                account.MobileNo = acctResult.Message.PhoneNumber;
                account.Status = acctResult.Message.AccountStatus;
                account.Email = acctResult.Message.Email;

                acctResult.ResponseCode = "00";
               
                try
                {
                    account.FirstName = acctResult.Message.Name.Split(' ')[0];
                    account.LastName = acctResult.Message.Name.Split(' ')[1];
                }
                catch (Exception)
                {

                }
                return acctResult;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return   new BankOneAccountSummaryModel() { ResponseCode = "06", ResponseMessage = "System Error" };
            }
        }

        public ResponseModel AccountEnquiry(string AccountNo)
        {
            try
            {
                var request = new { AccountNo = AccountNo, AuthenticationCode=BaseService.GetAppSetting("AuthToken") };
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/AccountEnquiry";
                var acctResult = apiservice.UrlPost<AccountEnquiryModel>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }              
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06","System Error");
            }
        }

        public ResponseModel FreezeAccount(FreezeAccountRequest request)
        {
            try
            {               
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/FreezeAccount";
                var acctResult = apiservice.UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel UnFreezeAccount(FreezeAccountRequest request)
        {
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/UnFreezeAccount";
                var acctResult = apiservice.UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel PlaceLien(PlaceLienModel request)
        {
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/PlaceLien";
                var acctResult = apiservice.UrlPost<BaseAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel UnPlaceLien(FreezeAccountRequest request)
        {
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/UnPlaceLien";
                var acctResult = apiservice.UrlPost<BaseAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel CheckPNDStatus(FreezeStatus request)
        {
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/CheckPNDStatus";
                var acctResult = apiservice.UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel ActivatePND(FreezeStatus request)
        {
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/ActivatePND";
                var acctResult = apiservice.UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel DeactivatePND(FreezeStatus request)
        {
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/DeActivatePND";
                var acctResult = apiservice.UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel CheckFreeze(FreezeStatus request)
        {
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/CheckFreezeStatus";
                var acctResult = apiservice.UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel CheckLienStatus(FreezeStatus request)
        {
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/CheckLienStatus";
                var acctResult = apiservice.UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel GetAllAccountsByAccountNo(string AccountNo, string CustomerId)
        {
            string MethodName = "GetAllAccountsByAccountNo";
            LogMachine.LogInformation(className, MethodName, $"entered method with paramters {AccountNo}; customerid {CustomerId}");
            
            try
            {
                var acctModel = new List<AccountDetailsModel>();
                //Get Account Details
                
                string sessionId = coreBankingService.GetCoreBankingSessionID(); LogMachine.LogInformation(className, MethodName, $"About getting session Id from core banking");
                if (string.IsNullOrEmpty(sessionId))
                {
                    LogMachine.LogInformation(className,MethodName,$"Unable to generate session id for user login {AccountNo}");
                    return ResponseDictionary.GetCodeDescription("06", "core banking authorization failed");
                }
                if (string.IsNullOrEmpty(CustomerId))
                {
                    LogMachine.LogInformation(className, MethodName, $"customer Id is empty about calling corebaking service to get customer Id ");
                    //Call web service to Get customer's ID
                    var custResult = GetCustomerByAccountNo(AccountNo,sessionId);
                    if (custResult.ResponseCode=="00")
                    {
                        CustomerId = custResult.ResultObject.ToString();
                    }
                }

                string Url = $"{AppConfig.CoreBankingBaseUrl}account/search?CustomerID={CustomerId}";
                var acctResult = apiservice.UrlGetWithRestSharp<CoreBankingListAccountModel>(Url, "", $"{AppConfig.AuthKey}:{sessionId}");
                if (acctResult == null)
                {
                    LogMachine.LogInformation(className, MethodName, $"no record gotten for account with account no ");
                    return ResponseDictionary.GetCodeDescription("04", "connection lost, no record found");
                }
                foreach (var item in acctResult.Payload)
                {
                    var acct = new AccountDetailsModel();                    
                    acct.AccountName = item.CustomerName;
                    acct.AccountNumber = item.AccountNumber;
                    acct.AccountType = item.AccountType;
                    acct.BranchId = item.BranchName;
                    acct.PhoneNumber = item.PhoneNumber;
                    acct.Status = item.Status;
                    acct.BvnNumber = item.BvnNumber;
                    acct.CustomerName = item.CustomerName;
                    acctModel.Add(acct);
                }
                LogMachine.LogInformation(className, MethodName, $"result gotten returning back to login {JsonConvert.SerializeObject(acctModel)} ");
                return ResponseDictionary.GetCodeDescription("00", "login successful", acctModel);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetCustomerByAccountNo(string AccountNo, string sessionid)
        {
            string MethodName = "GetCustomerByAccountNo";
            LogMachine.LogInformation(className, MethodName, $"entered method with paramters accpunt no {AccountNo}");
            try
            {
                string Url = $"{AppConfig.CoreBankingBaseUrl}customer/{AccountNo}/basicInfo";
                LogMachine.LogInformation(className, MethodName, $"calling service {Url} ");
                var acctResult = apiservice.UrlGetWithRestSharp<CoreBankingBasicAccountInfoModel>(Url, "", $"{AppConfig.AuthKey}:{sessionid}");
                if (acctResult == null)
                {
                    LogMachine.LogInformation(className, MethodName, $"connection lost, no result no result from core banking ");
                    return ResponseDictionary.GetCodeDescription("04", "connection lost, no record found");
                }
                if (acctResult.Payload.CustomerId>0)
                {
                    LogMachine.LogInformation(className, MethodName, $"result is {JsonConvert.SerializeObject(acctResult)} ");
                    return ResponseDictionary.GetCodeDescription("00", acctResult.Payload.CustomerId);
                }
                return ResponseDictionary.GetCodeDescription("06");
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(className, MethodName, $"an error occurred  {ex.ToString()}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public StatementBaseResponse GetStatement(TransactionRequest request)
        {
            try
            {
                TransactionBaseResponse transactionBaseResponse = new TransactionBaseResponse();
                //Get Account Details              
                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GenerateAccountStatement2/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={request.AccountNumber}&fromDate={request.StartDate}&toDate={request.EndDate}&isPdf=false&arrangeAsc=true&showSerialNumber=true&showTransactionDate=true&showReversedTransactions=false&showInstrumentNumber=true&institutionCode={BaseService.GetAppSetting("BankOneCode")}";
                var acctResult = apiservice.UrlGet<TransactionBaseResponse>(Url, "");
                if (acctResult == null)
                {
                    return new StatementBaseResponse { ResponseCode = "05", ResponseMessage = "no response from server" };                   
                }
                if (acctResult.IsSuccessful == false)
                {
                    return new StatementBaseResponse { ResponseCode = "04", ResponseMessage = acctResult.Message };
                   
                }
                return new StatementBaseResponse { ResponseCode = "00", ResponseMessage = acctResult.Message };
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return new StatementBaseResponse { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
        }

        public ResponseModel GetTransaction(TransactionHistoryBaseRequest request)
        {
            BankoneTransactionHistoryResponse historyResponse = new BankoneTransactionHistoryResponse();
            TransactionBaseResponse secondBaseResponse = new TransactionBaseResponse();
            try
            {
           // /Account/GetTransactions/2?authtoken=51d61612-02c2-4b28-b830-aa5c79bc4ddb&accountNumber=1100146917&fromDate=01-02-2021&toDate=20-03-2021&institutionCode=100604&numberOfItems=1
                                         
                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GetTransactions/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={request.AccountNumber}&fromDate={request.StartDate}&toDate={request.EndDate}&institutionCode={BaseService.GetAppSetting("BankOneCode")}&numberOfItems={request.NumberOfItems}";
                var stringResult = apiservice.UrlGet(Url, "");

                if (string.IsNullOrEmpty(stringResult))
                {
                    return ResponseDictionary.GetCodeDescription("04", " error fetching records, pleas retry");
                }

                try
                {
                    historyResponse = JsonConvert.DeserializeObject<BankoneTransactionHistoryResponse>(stringResult);
                }
                catch (Exception ex)
                {                  
                   //extract second response 
                   secondBaseResponse = JsonConvert.DeserializeObject<TransactionBaseResponse>(stringResult);
                   return ResponseDictionary.GetCodeDescription("04", secondBaseResponse.Message);                    
                }

               
                if (historyResponse.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", historyResponse.Message);
                }
                if (historyResponse.Message.Count<=0)
                {
                    return ResponseDictionary.GetCodeDescription("04", "No record found for date range");
                }

                return ResponseDictionary.GetCodeDescription("00", historyResponse.Message);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetProducts()
        {
            try
            {              

                string Url = $"{AppConfig.CoreBankingBaseUrl}Product/Get/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&mfbCode={BaseService.GetAppSetting("BankOneCode")}";
                var prdResult = apiservice.UrlGet<List<ProductModel>>(Url, "");
                if (prdResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", "no record found");
                }
                if (prdResult.Count <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04", "product code not found");
                }
                return ResponseDictionary.GetCodeDescription("00", prdResult);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetBankOneBanks()
        {
            try
            {

                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}BillsPayment/GetCommercialBanks/{BaseService.GetAppSetting("AuthToken")}";
                var prdResult = apiservice.UrlGet<List<BankModel>>(Url, "");
                if (prdResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", "no record found");
                }
                if (prdResult.Count <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04", "product code not found");
                }
                var banks = new List<GenericResponseModel>();
                foreach (var item in prdResult)
                {
                    banks.Add(new GenericResponseModel() { Name=item.Name, Code=item.Code });
                }
                return ResponseDictionary.GetCodeDescription("00", banks);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel ValidateBVN(string BVN)
        {
            try
            {
                var bvnRequest = new BVNRequest() { BVN=BVN, Token=BaseService.GetAppSetting("AuthToken") };

                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}account/bvn/getbvndetails";
                var bvnResult = apiservice.UrlPost<BVNResponse>(Url, bvnRequest);
                if (bvnResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", "no record found");
                }
                if (bvnResult.isBvnValid == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", "Invalid BVN");
                }
                return ResponseDictionary.GetCodeDescription("00", bvnResult.bvnDetails);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
    }
}
