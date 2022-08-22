using ServicBus.Logic.Contracts;
using ServiceBus.Core;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Contracts.BankOne;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.AccountResult;
using ServiceBus.Logic.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class BankOneAccountCreationIntegration: IBankOneAccountCreationIntegration
    {
        string classname = "BankAccountCreationIntegration";
        IBankOneCoreBankingAuthentication coreBankingService;
        IApiPostAndGet apiPostAndGet;
        public BankOneAccountCreationIntegration(IBankOneCoreBankingAuthentication coreBanking, IApiPostAndGet api)
        {
            coreBankingService = coreBanking;
            apiPostAndGet = api;
        }
        public AccountCreationResponse CreateNewAccount(Account request)
        {
            string methodname = "CreateNewAccount";
           
            LogService.LogInfo(request.OperatorId,classname, methodname, "inside method about processing ");
            try
            {
                //create account 
                LogService.LogInfo(request.OperatorId,classname, methodname, $"about creating customer");
                return CreateAccountInfo(request);

            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, classname, methodname, ex);
                return
                    new
                    AccountCreationResponse()
                    { ResponseCode = "96", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankId };
            }
        }

        public AccountCreationResponse CreateAccountInfo(Account request)
        {
            string methodname = "CreateAccountInfo";
            try
            {
                string accountGuid = Guid.NewGuid().ToString();
                
                LogService.LogInfo(request.OperatorId,classname, methodname, $"entered method ");
                #region account model parsing
                var accountRequest = new BankOneAccountCreationApiRequest()
                {
                    AccountInformationSource = 0,
                    AccountNumber = "",
                    AccountOfficerCode = string.IsNullOrEmpty(request.AccountOfficerCode) ? BaseService.GetAppSetting("AccountOfficerCode") : request.AccountOfficerCode,
                    AccountOpeningTrackingRef = accountGuid,
                    Address = request.ResidentialAddress,
                    BVN = request.BVN,
                    CustomerID = "",
                    CustomerImage = request.Passport,
                    CustomerSignature = request.Signature,
                    DateOfBirth = request.DOB,
                    Email = request.Email,
                    FullName = $"{request.FirstName} {request.LastName}",
                    Gender = request.Gender,
                    HasSufficientInfoOnAccountInfo = true,
                    LastName = request.LastName,
                    NationalIdentityNo = request.NIN,
                    NextOfKinName = request.NOKName,
                    NextOfKinPhoneNo = request.NOKNo,
                    NotificationPreference = 0,
                    OtherAccountInformationSource = "",
                    OtherNames = request.FirstName + " " + request.MiddleName,
                    PhoneNo = request.MobileNo,
                    PlaceOfBirth = request.ResidentialState,
                    ProductCode = string.IsNullOrEmpty(request.ProductCode) ? BaseService.GetAppSetting("ProductCode") : request.ProductCode,
                    ReferralName = request.ReferralName,
                    ReferralPhoneNo = request.ReferralPhoneNo,
                    TransactionPermission = 0,
                    TransactionTrackingRef = accountGuid
                };

                #endregion

                string acctUrl = string.Concat(BaseService.GetAppSetting("CoreBankingBaseUrl"), BaseService.GetAppSetting("AccountCreationEndPoint"), BaseService.GetAppSetting("ApiVersion"), "?authtoken=", BaseService.GetAppSetting("AuthToken"));

                LogService.LogInfo(request.OperatorId,classname, methodname, $"about creating account  {accountRequest} {acctUrl}");

                var acctResult = apiPostAndGet.UrlPost<AccountCreationApiResultModel>(acctUrl, accountRequest);

                LogService.LogInfo(request.OperatorId, classname, methodname, $"account info result {acctResult} ");
                if (acctResult == null)
                {
                    return
                      new
                      AccountCreationResponse()
                      { ResponseCode = "06", ResponseMessage = "No response from server", OperatorId = request.OperatorId, BankId = request.BankId };
                }
                if (acctResult.IsSuccessful == false)
                {
                  return
                    new
                    AccountCreationResponse()
                    { ResponseCode = "06", ResponseMessage = acctResult.Message.CreationMessage, OperatorId = request.OperatorId, BankId = request.BankId };
                }
                request.AccountNumber = acctResult.Message.AccountNumber;
                request.CustomerId = acctResult.Message.CustomerID;

             
                LogService.LogInfo(request.OperatorId, classname, methodname, $"account creation sccesfull returning ");
              
                return
                  new
                  AccountCreationResponse()
                         { ResponseCode = "06", ResponseMessage = "Account creation was successful", OperatorId = request.OperatorId, BankId = request.BankId,
                         AccountNumber= acctResult.Message.AccountNumber, 
                         CustomerID= acctResult.Message.CustomerID
                         };

            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, classname, methodname, ex);
                return
                    new
                    AccountCreationResponse()
                    { ResponseCode = "96", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankId };
            }
        }
    }
}
