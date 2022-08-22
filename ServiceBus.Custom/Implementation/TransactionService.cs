using ServicBus.Logic.Implementations;
using ServicBus.Logic.Implementations.IO.Image;
using ServicBus.Logic.Implementations.Memory;
using ServiceBus.Core;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Custom.Contract;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http;

namespace ServiceBus.Custom.Implementation
{
    /// <summary>
    /// esb transaction service
    /// </summary>
    public class TransactionService : ITransactionService
    {
        IPostingIntegrationService postingService;

        string className = "TransactionService";
        /// <summary>
        /// initializing the transaction service
        /// </summary>
        /// <param name="node"></param>
        public TransactionService(IPostingIntegrationService posting)
        {
            postingService = posting;
        }

        public ResponseModel CreditAccount(PostingModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccountNumber))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Invalid account No");
                }
                if (string.IsNullOrEmpty(request.RetrievalReference))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Invalid reference No");
                }
                if (request.Amount < 0)
                {
                    return ResponseDictionary.GetCodeDescription("04", "Amount cannot be less then Zero");
                }
                if (string.IsNullOrEmpty(request.Narration))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Narration is required");
                }
                if (string.IsNullOrEmpty(request.Token))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Token is required");
                }
                if (request.Token.Trim()!=BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("01", "Token mismatch");
                }
                request.NibssCode = BaseService.GetAppSetting("BankOneCode");
                //check pin
                using (AiroPayContext context = new AiroPayContext())
                {
                    var mobileAcct = context.Account.Where(x => x.AccountNumber == request.AccountNumber).FirstOrDefault();
                    if (mobileAcct == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "account number is not registered on this channel");
                    }                   
                   
                    return postingService.CreditAccount(request);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06","System Error");
            }
        }

        public ResponseModel DebitAccount(PostingModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccountNumber))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Invalid account No");
                }
                if (string.IsNullOrEmpty(request.RetrievalReference))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Invalid reference No");
                }
                if (request.Amount < 0)
                {
                    return ResponseDictionary.GetCodeDescription("04", "Amount cannot be less then Zero");
                }
                if (string.IsNullOrEmpty(request.Narration))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Narration is required");
                }
                if (string.IsNullOrEmpty(request.Token))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Token is required");
                }
                if (request.Token.Trim() != BaseService.GetAppSetting("AuthToken"))
                {
                    return ResponseDictionary.GetCodeDescription("01", "Token mismatch");
                }
                request.NibssCode = BaseService.GetAppSetting("BankOneCode");
                //check pin
                using (AiroPayContext context = new AiroPayContext())
                {
                    var mobileAcct = context.Account.Where(x => x.AccountNumber == request.AccountNumber).FirstOrDefault();
                    if (mobileAcct == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "account number is not registered on this channel");
                    }

                    return postingService.DebitAccount(request);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }              

        public ReversalResponse Reversal(ReversalRequest request)
        {
            try
            {          
                return postingService.Reversal(request);                
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return new ReversalResponse { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
        }

        public TransactionQueryResponse TransactionQuery(TransactionQueryRequest request)
        {
            string methodName = "TransactionQuery";
            try
            {
                if (string.IsNullOrEmpty(request.TransactionType))
                {
                    return new TransactionQueryResponse { ResponseCode= "01", ResponseMessage= "Invalid transaction type" };
                }
                if (string.IsNullOrEmpty(request.RetrievalReference))
                {
                    return new TransactionQueryResponse { ResponseCode = "01", ResponseMessage = "Invalid reference no" };
                   
                }
                if (request.Amount < 0)
                {
                    return new TransactionQueryResponse { ResponseCode = "01", ResponseMessage = "inavlid amount" };
                }               
               
                return postingService.TransactionQuery(request);
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return new TransactionQueryResponse { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
        }
    }
}
