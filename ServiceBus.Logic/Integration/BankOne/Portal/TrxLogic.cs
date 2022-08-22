using ServicBus.Logic.Implementations;
using ServiceBus.Core;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration.Portal
{
   public  class TrxLogic
    {
       static  string Classname = "TrxLogic";
        public static ResponseModel Reversal(ReversalApiRequest request)
        {
            string method = "Reversal";
            LogMachine.LogInformation(Classname, method, $"entered the reversal service");

            request.Token = BaseService.GetAppSetting("AuthToken");
            try
            {
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}CoreTransactions/Reversal";
                var postingResult = new ApiPostAndGet().UrlPost<ReversalResponse>(Url, request);
                if (postingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, reversal failed");
                }
                if (postingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("06", " no response from server, reversal failed");
                }
                if (postingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", postingResult.ResponseMessage);
                }
                using (AiroPayContext context = new AiroPayContext())
                {
                    var dbRequest = new Reversal()
                    {

                        Amount = request.Amount,
                        TransactionDate = DateTime.Now.ToString(),
                        TransactionType = "Reversal",
                        DateCreated = DateTime.Now,
                        RetrievalReference = request.RetrievalReference,
                        ResponseCode = postingResult.ResponseCode,
                        ResponseMessage = postingResult.ResponseMessage
                    };
                    context.Reversal.Add(dbRequest);
                    context.SaveChanges();
                }
                return ResponseDictionary.GetCodeDescription(postingResult.ResponseCode, postingResult.ResponseMessage);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel TransactionQuery(TransactionQueryApiRequest request)
        {
            try
            {
              //  request.Token = BaseService.GetAppSetting("AuthToken");

                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}CoreTransactions/TransactionStatusQuery ";
                var acctResult = new ApiPostAndGet().UrlPost<TransactionQueryResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " connection error");
                }
                if (acctResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription(acctResult.ResponseCode, acctResult.ResponseMessage);
                }
              
                return ResponseDictionary.GetCodeDescription(acctResult.ResponseCode, acctResult.ResponseMessage, acctResult.Status);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System / Request Error");
            }
        }

        public static ResponseModel PlaceLien(PlaceLienModel request)
        {
            try
            {
                using (AiroPayContext context=new AiroPayContext())
                {
                    request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");

                    string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/PlaceLien";
                    var acctResult = new ApiPostAndGet().UrlPost<BaseAccountResponse>(Url, request);
                    if (acctResult == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", " no record found");
                    }
                    if (acctResult.RequestStatus == false)
                    {
                        return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                    }
                    var log = new LienLog();
                    log.AccountNo = request.AccountNo;
                    log.Amount = request.Amount;
                    log.AuthenticationCode = request.AuthenticationCode;
                    log.Reason = request.Reason;
                    log.ReferenceID = request.ReferenceID;
                    log.ResponseCode = acctResult.ResponseDescription;
                    log.ResponseMessage = acctResult.ResponseStatus;
                    log.Status = "Liened";
                    context.LienLog.Add(log);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription, acctResult.ResponseStatus);

                }
               
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel UnPlaceLien(FreezeAccountRequest request)
        {
            try
            {
                using (AiroPayContext context = new AiroPayContext())
                {
                    request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");

                    string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/UnPlaceLien";
                    var acctResult = new ApiPostAndGet().UrlPost<BaseAccountResponse>(Url, request);
                    if (acctResult == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", " no record found");
                    }
                    if (acctResult.RequestStatus == false)
                    {
                        return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                    }
                    var lienData = context.LienLog.Where(x => x.ReferenceID == request.ReferenceID).FirstOrDefault();
                    if (lienData!=null)
                    {
                        lienData.Status = "Un-Liened";                       
                        context.SaveChanges();
                    }
                    return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription, acctResult.ResponseStatus);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel CheckLienStatus(FreezeStatus request)
        {
            try
            {
                request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/CheckLienStatus";
                var acctResult = new ApiPostAndGet().UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription, acctResult.ResponseStatus);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel FreezeAccount(FreezeAccountRequest request)
        {
            try
            {
                request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/FreezeAccount";
                var acctResult = new ApiPostAndGet().UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel UnFreezeAccount(FreezeAccountRequest request)
        {
            try
            {
                request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/UnFreezeAccount";
                var acctResult = new ApiPostAndGet().UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel CheckPNDStatus(FreezeStatus request)
        {
            try
            {
                request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/CheckPNDStatus";
                var acctResult = new ApiPostAndGet().UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel ActivatePND(FreezeStatus request)
        {
            try
            {
                request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/ActivatePND";
                var acctResult = new ApiPostAndGet().UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel DeactivatePND(FreezeStatus request)
        {
            try
            {
                request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/DeActivatePND";
                var acctResult = new ApiPostAndGet().UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public static ResponseModel CheckFreeze(FreezeStatus request)
        {
            try
            {
                request.AuthenticationCode = BaseService.GetAppSetting("AuthToken");
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}Account/CheckFreezeStatus";
                var acctResult = new ApiPostAndGet().UrlPost<FreezeAccountResponse>(Url, request);
                if (acctResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                if (acctResult.RequestStatus == false)
                {
                    return ResponseDictionary.GetCodeDescription("04", acctResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", acctResult.ResponseDescription);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred for user authentication {ex.Message}; {ex.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }
    }
}
