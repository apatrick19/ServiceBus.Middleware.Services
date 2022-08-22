using Newtonsoft.Json;
using ServiceBus.Core;
using ServiceBus.Core.ControllerModel;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.AccountResult;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration.Portal
{
   public  class AccountLogic
    {
        static string classname = "AccountLogic";
        public static AccountCreationResponse CreateAccount(Account account)
        {
             string methodname = "CreateAccount";
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    LogMachine.LogInformation(classname, methodname, $"checking db to see if account exists {account.MobileNo}");
                    var existing = context.Account.Where(x => x.MobileNo == account.MobileNo).FirstOrDefault();
                    if (existing == null)
                    {
                //call corebanking service                        
                   
                string accountGuid = Guid.NewGuid().ToString();
              
                 #region account model parsing
                var accountRequest = new BankOneAccountCreationApiRequest()
                {
                    AccountInformationSource = 0,
                    AccountNumber = "",
                    AccountOfficerCode = string.IsNullOrEmpty(account.AccountOfficerCode) ? BaseService.GetAppSetting("AccountOfficerCode") : account.AccountOfficerCode,
                    AccountOpeningTrackingRef = accountGuid,
                    Address = account.ResidentialAddress,
                    BVN = account.BVN??string.Empty,
                    CustomerID = "",
                    CustomerImage = string.Empty,
                    CustomerSignature = string.Empty,
                    DateOfBirth = account.DOB,
                    Email = account.Email,
                    FullName = $"{account.FirstName} {account.LastName}",
                    Gender = account.Gender,
                    HasSufficientInfoOnAccountInfo = true,
                    LastName = account.LastName,
                    NationalIdentityNo = account.NIN??string.Empty,
                    NextOfKinName = account.NOKName??string.Empty,
                    NextOfKinPhoneNo = account.NOKNo??string.Empty,
                    NotificationPreference = 0,
                    OtherAccountInformationSource = "",
                    OtherNames = account.FirstName + " " + account.MiddleName,
                    PhoneNo = account.MobileNo,
                    PlaceOfBirth = account.ResidentialState,
                    ProductCode = string.IsNullOrEmpty(account.ProductCode) ? BaseService.GetAppSetting("ProductCode") : account.ProductCode,
                    ReferralName = string.Empty,
                    ReferralPhoneNo = string.Empty,
                    TransactionPermission = 0,
                    TransactionTrackingRef = accountGuid
                };
                #endregion

                string acctUrl = string.Concat(BaseService.GetAppSetting("CoreBankingBaseUrl"), BaseService.GetAppSetting("AccountCreationEndPoint"), BaseService.GetAppSetting("ApiVersion"), "?authtoken=", BaseService.GetAppSetting("AuthToken"));

                LogMachine.LogInformation(classname, methodname, $"about creating account  {accountRequest} {acctUrl}");
               
                var acctResult = RestPostRequestIntegration.APICall<AccountCreationApiResultModel>(accountRequest, acctUrl);

                LogMachine.LogInformation(classname, methodname, $"account info result {acctResult} ");
                if (acctResult == null)
                {
               
                            return new AccountCreationResponse() { ResponseCode = "06", ResponseMessage = "account creation failed" };
                }
                if (acctResult.IsSuccessful == false)
                {
                            //process with hangfire
                            return new AccountCreationResponse() { ResponseCode = "06", ResponseMessage = acctResult.Message.CreationMessage };
                            
                }
                account.AccountNumber = acctResult.Message.AccountNumber;
                account.CustomerId = acctResult.Message.CustomerID;
                account.Status = "0";
                account.StatusName = "Account Approved";

                var result = new AccountCreationResponse();
                result.AccountNo = account.AccountNumber;
                result.CustomerID = account.CustomerId;
              
                context.Account.Add(account);
                context.SaveChanges();
                        result.ResponseCode = "00";
                result.ResponseMessage  = $"Account creation was successful; Account No: {account.AccountNumber}; Customer ID: {account.CustomerId}";
                LogMachine.LogInformation(classname, methodname, $"account creation sccesfull returning ");
                      return  result;
               
                }
                  
                    return new AccountCreationResponse() { ResponseCode = "44", ResponseMessage = "Account exists with mobile no, kindly inform " };
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return new AccountCreationResponse() { ResponseCode="06" ,ResponseMessage="System Malfunction, contact admin"};
            }
        }

        public static AccountCreationResponse UpdateAccount(Account account)
        {
            string methodname = "UpdateAccount";
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    LogMachine.LogInformation(classname, methodname, $"checking db to see if account exists {account.MobileNo}");
                    var existing = context.Account.Where(x => x.ID == account.ID).FirstOrDefault();
                    if (existing != null)
                    {
                        var result = new AccountCreationResponse();
                        account.AccountNumber = existing.AccountNumber;
                        account.CustomerId = existing.CustomerId;
                        account.ID = existing.ID;

                        context.Entry(existing).CurrentValues.SetValues(account);
                        context.SaveChanges();
                        result.ResponseCode = "00";
                        result.ResponseMessage = $"Account update was successful";
                        LogMachine.LogInformation(classname, methodname, $"account creation sccesfull returning ");
                        return result;

                    }                  
                    return new AccountCreationResponse() { ResponseCode = "04", ResponseMessage = "Account update Failed " };
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return new AccountCreationResponse() { ResponseCode = "06", ResponseMessage = "System Malfunction, contact admin" };
            }
        }

        public static BankOneAccountSummaryModel RegisterExistingAccount(Account account)
        {
            string methodname = "RegisterExistingAccount";
            LogMachine.LogInformation(classname, methodname, "inside method about processing ");
            try
            {
                LogMachine.LogInformation(classname, methodname, "mapping models ");
                using (AiroPayContext context = new AiroPayContext())
                {
                    LogMachine.LogInformation(classname, methodname, $"checking db to see if account exists {account.MobileNo}");
                    var existing = context.Account.Where(x => x.AccountNumber == account.AccountNumber).FirstOrDefault();
                    if (existing == null)
                    {
                        //validate account no  
                        LogMachine.LogInformation(classname, methodname, "account has not been registered, about parsing to corebaking methods for account validation ");
                        var AcctResult = AccountValidationService.GetAccountByAccountNoStatic(account);
                        if (AcctResult.ResponseCode == "00")
                        {
                            account.Status = "0";
                            account.StatusName = "Account Registered";
                            context.Account.Add(account);
                            context.SaveChanges();
                            AcctResult.ResponseMessage = "Account has been successfully registered; please provide OTP sent to the customer for account activation on the service";
                            return AcctResult;
                        }
                        else
                        {
                            return AcctResult;
                        }

                    }
                    return new BankOneAccountSummaryModel() { ResponseCode = "44", ResponseMessage = "Account exists on this service, kindly inform customer to login" };
                   
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}");
                return new BankOneAccountSummaryModel() { ResponseCode = "06", ResponseMessage = "System Error, contact admin" };
            }
        }

        public static BankOneAccountSummaryModel ActivateAccount(string AccountNumber, string OTP)
        {
            string methodname = "RegisterExistingAccount";
            LogMachine.LogInformation(classname, methodname, "inside method about processing ");
            try
            {
                LogMachine.LogInformation(classname, methodname, "mapping models ");
                using (AiroPayContext context = new AiroPayContext())
                {
                 
                    var existing = context.Account.Where(x => x.AccountNumber == AccountNumber).FirstOrDefault();
                    if (existing != null)
                    {

                        existing.Status = "0";
                        existing.StatusName = "Account Activated";                           
                        context.SaveChanges();
                        return new BankOneAccountSummaryModel() { ResponseMessage = "Account activation complete; default Password and PIN has been shared with the customer" , ResponseCode="00"};
                    }
                    return new BankOneAccountSummaryModel() { ResponseCode = "44", ResponseMessage = "Oppss! Record not found for activation, please restart process." };

                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred {ex}");
                return new BankOneAccountSummaryModel() { ResponseCode = "06", ResponseMessage = "System Error, contact admin" };
            }
        }
    }
}
