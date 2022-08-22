using ServicBus.Logic.Implementations;
using ServiceBus.Core;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Settings;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.PortalModel;
using ServiceBus.Logic.Model.Transfer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration.Portal
{
    public class TransferLogic
    {
        static string classname = "TransferLogic";
        public static LocalTransferNameEnquiryResponse LocalTransferNameEnquiry(TransferNameEnquiryRequest request)
        {
            string methodname = "LocalTransferNameEnquiry";
            try
            {
                if (string.IsNullOrEmpty(request.SourceAccountNo))
                {
                    return new LocalTransferNameEnquiryResponse() { ResponseCode = "06", ResponseMessage = "Invalid Payer Account No" };
                }

                if (string.IsNullOrEmpty(request.DestinationAccountNo))
                {
                    return new LocalTransferNameEnquiryResponse() { ResponseCode = "06", ResponseMessage = "Invalid Recipient Account No" };
                }

                var result = new LocalTransferNameEnquiryResponse();
                result.SourceAccount = GetAccountByAccountNo(request.SourceAccountNo);
                if (result.SourceAccount.ResponseCode != "00")
                {
                    result.ResponseCode = result.SourceAccount.ResponseCode;
                    result.ResponseMessage = "Invalid Payer Account Number";
                    return result;
                }
                result.DestinationAccount = GetAccountByAccountNo(request.DestinationAccountNo);               
                
                if (result.DestinationAccount.ResponseCode != "00")
                {
                    result.ResponseCode = result.DestinationAccount.ResponseCode;
                    result.ResponseMessage = "Invalid Recipient Account Number";
                    return result;
                }
                 result.ReferenceNo = DateTime.Now.ToString("yyMMddHHmmss");
                 result.ResponseCode = "00";
                 result.ResponseMessage = "Name Validation successful";
                 return result;
                
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, methodname, "",ex);
                return new LocalTransferNameEnquiryResponse() { ResponseCode = "06", ResponseMessage = "System Error" };
            }
        }

        public static BankOneAccountSummaryModel GetAccountByAccountNo(string AccountNumber)
        {
            try
            {

                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GetAccountSummary/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={AccountNumber}&institutionCode={BaseService.GetAppSetting("BankOneCode")}";
                var acctResult = new ApiPostAndGet().UrlGet<BankOneAccountSummaryModel>(Url, "");
                if (acctResult == null)
                {
                    return new BankOneAccountSummaryModel() { ResponseCode = "04", ResponseMessage = "system error, please retry again later" };

                }
                if (acctResult.IsSuccessful == false)
                {
                    return new BankOneAccountSummaryModel() { ResponseCode = "04", ResponseMessage = "no record found" };
                }
                acctResult.ResponseCode = "00";
                acctResult.ResponseMessage = "Successful";
                return acctResult;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return new BankOneAccountSummaryModel() { ResponseCode = "06", ResponseMessage = "System Error" };
            }
        }

        public static ResponseModel LocalTransfer(LocalTransferApiRequest request)
        {
            try
            {
                request.Amount = request.Amount * 100;
                request.AuthenticationKey = BaseService.GetAppSetting("AuthToken");
                request.Fee = 0;

                using (AiroPayContext context = new AiroPayContext())
                {
                    string Url = string.Concat(BaseService.GetAppSetting("ThirdPartyBankingBaseUrl"), "CoreTransactions/LocalFundsTransfer");
                    var response = new ApiPostAndGet().UrlPost<LocalTransferResponse>(Url, request);
                    if (response == null)
                    {
                        return ResponseDictionary.GetCodeDescription("06", "No response from BankOne");
                    }
                    if (response.ResponseCode != "00")
                    {
                        return ResponseDictionary.GetCodeDescription("06", response.ResponseMessage);
                    }                
                    var dbModel = new Transfer()
                    {
                        Amount = request.Amount,
                        BeneficiaryAccountNumber = request.ToAccountNumber,
                        DestinationBankCode = BaseService.GetAppSetting("BankOneCode"),
                        Narration = request.Narration,
                        DateCreated = DateTime.Now,
                        PaymentReference = request.RetrievalReference,
                        SourceAccountNo = request.FromAccountNumber,
                        SessionId = request.RetrievalReference,
                        SourceBankCode = BaseService.GetAppSetting("BankOneCode"),
                        TransferType = "LocalTransfer",
                        ResponseCode = response?.ResponseCode,
                        ResponseMessage = response?.ResponseMessage
                    };
                    context.Transfer.Add(dbModel);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription(response.ResponseCode, response.ResponseMessage, response.Reference);
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static NameEnquiryResponse NameEnquiry(NameEnquiryRequest request)
        {
            string method = "NameEnquiry";
            var response = new ResponseModel();
            LogMachine.LogInformation(classname, method, $"entered the service");
            try
            {
              
              
                string Url = string.Concat(BaseService.GetAppSetting("ThirdPartyBankingBaseUrl"), "Transfer/NameEnquiry");
                var result = new ApiPostAndGet().UrlPost<NameEnquiryResponse>(Url, request);
                if (result==null)
                {
                    return new NameEnquiryResponse() { ResponseCode = "06", ResponseMessage = result.ResponseMessage };
                }
                if (result.IsSuccessful == false)
                {
                    return new NameEnquiryResponse() { ResponseCode= "06", ResponseMessage  =result.ResponseMessage };
                }
                result.InstitutionCode = request.BankCode;
                result.AccountNumber = request.AccountNumber;
                result.ResponseCode = "00";
                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, method, $"an error occurred {ex}");                
                return new NameEnquiryResponse() { ResponseCode = "06", ResponseMessage = "application error, could not get name" };
            }
        }

        public static ResponseModel InterBankTransfer(FundTransferApiRequest Request)
        {
            string method = "FundTransfer";
            var response = new ResponseModel();
            LogMachine.LogInformation(classname, method, $"entered the service");
            try
            {
                Request.ReceiverAccountType = "SavingsOrCurrent";
                string Url = string.Concat(BaseService.GetAppSetting("ThirdPartyBankingBaseUrl"), "Transfer/InterbankTransfer");

                var result = RestPostRequestIntegration.APICall<FundTransferApiResponse>(Request, Url);
                if (result.IsSuccessFul == false)
                {
                    return ResponseDictionary.GetCodeDescription(result.ResponseCode, "Transaction Failed, Error from BankOne");
                }
                return ResponseDictionary.GetCodeDescription(result.ResponseCode, result.ResponseMessage, result.ReferenceID);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, method, $"an error occurred {ex}");
                response.ResponseCode = "06";
                response.ResponseDescription = "application error, could not get name";
                return response;
            }
        }
    }
}
