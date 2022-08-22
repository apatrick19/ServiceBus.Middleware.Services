using Newtonsoft.Json;
using ServicBus.Logic.Contracts;
using ServicBus.Logic.Implementations.Memory;
using ServiceBus.Core;
using ServiceBus.Core.ControllerModel;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Settings;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.AccountResult;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class AccountCreationService : IAccountCreationService
    {
        string classname = "AccountCreationService";
        IBankOneCoreBankingAuthentication coreBankingService;
        IApiPostAndGet apiPostAndGet;
        public AccountCreationService(IBankOneCoreBankingAuthentication coreBanking, IApiPostAndGet api)
        {
            coreBankingService = coreBanking;
            apiPostAndGet = api;
        }
        public ResponseModel CreateNewAccount(Account account)
        {
            string methodname = "CreateNewAccount";
            LogMachine.LogInformation(classname, methodname, "inside method about processing ");
            try
            {             
                //create account 
                LogMachine.LogInformation(classname, methodname, $"about creating customer");
                return CreateAccountInfo(account);
                
               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel CreateAccountInfo(Account account)
        {
            try
            {
                string accountGuid = Guid.NewGuid().ToString();
                string methodname = "CreateAccountInfo";
                LogMachine.LogInformation(classname, methodname, $"entered method ");
                #region account model parsing
                var accountRequest = new BankOneAccountCreationApiRequest()
                {
                    AccountInformationSource = 0,
                    AccountNumber = "",
                    AccountOfficerCode = string.IsNullOrEmpty(account.AccountOfficerCode) ? BaseService.GetAppSetting("AccountOfficerCode"): account.AccountOfficerCode,
                    AccountOpeningTrackingRef = accountGuid,
                    Address = account.ResidentialAddress,
                    BVN = account.BVN,
                    CustomerID = "",
                    CustomerImage = account.Passport,
                    CustomerSignature = account.Signature,
                    DateOfBirth = account.DOB,
                    Email = account.Email,
                    FullName = $"{account.FirstName} {account.LastName}",
                    Gender = account.Gender,
                    HasSufficientInfoOnAccountInfo = true,
                    LastName = account.LastName,
                    NationalIdentityNo = account.NIN,
                    NextOfKinName = account.NOKName,
                    NextOfKinPhoneNo = account.NOKNo,
                    NotificationPreference = 0,
                    OtherAccountInformationSource = "",
                    OtherNames = account.FirstName + " " + account.MiddleName,
                    PhoneNo = account.MobileNo,
                    PlaceOfBirth = account.ResidentialState,
                    ProductCode = string.IsNullOrEmpty(account.ProductCode) ? BaseService.GetAppSetting("ProductCode") : account.ProductCode,
                    ReferralName = account.ReferralName,
                    ReferralPhoneNo = account.ReferralPhoneNo,
                    TransactionPermission = 0,
                    TransactionTrackingRef = accountGuid
                };

                #endregion
               
                string acctUrl = string.Concat(BaseService.GetAppSetting("CoreBankingBaseUrl"), BaseService.GetAppSetting("AccountCreationEndPoint"), BaseService.GetAppSetting("ApiVersion"), "?authtoken=", BaseService.GetAppSetting("AuthToken"));

                LogMachine.LogInformation(classname, methodname, $"about creating account  {accountRequest} {acctUrl}");

                var acctResult = apiPostAndGet.UrlPost<AccountCreationApiResultModel>(acctUrl, accountRequest);

                LogMachine.LogInformation(classname, methodname, $"account info result {acctResult} ");
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("06", "account creation failed");
                }
                if (acctResult.IsSuccessful == false)
                {
                    //process with hangfire
                    return ResponseDictionary.GetCodeDescription("06", acctResult.Message.CreationMessage);
                }
                account.AccountNumber = acctResult.Message.AccountNumber;
                account.CustomerId = acctResult.Message.CustomerID;

                var result = new NewAccountResult();
                result.AccountNo = account.AccountNumber;
                result.CustomerID = account.CustomerId;
                //result.AccountType = "Voluntary Savings Account";
                LogMachine.LogInformation(classname, methodname, $"account creation sccesfull returning ");
                return ResponseDictionary.GetCodeDescription("00", "Account creation was successful", result);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
    }
}
