using Newtonsoft.Json;
using ServicBus.Logic.Contracts;
using ServiceBus.Core;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Core.Settings;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.Postings;
using ServiceBus.Logic.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class PostingIntegrationService : IPostingIntegrationService
    {
      
        IApiPostAndGet apiservice;
        string Classname = "PostingIntegrationService";
        public PostingIntegrationService(IApiPostAndGet apiPostAndGet )
        {
           
            apiservice = apiPostAndGet;
        }
       
        public ResponseModel CreditAccount(PostingModel model)
        {
            string method = "CreditAccount";
            LogMachine.LogInformation(Classname, method, $"entered the service for crediting account " + model.AccountNumber);
            try
            {

                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}/CoreTransactions/Credit";
                var postingResult = apiservice.UrlPost<PostingResponse>(Url, model);
                if (postingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, debit failed");
                }
                if (postingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("06", " no response from server, debit failed");
                }
                if (postingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", postingResult.ResponseMessage);
                }
                using (AiroPayContext context=new AiroPayContext())
                {
                    var dbRequest = new Transactions() {
                        AccountNo = model.AccountNumber,
                        Amount = model.Amount, ApplicationRefNo = model.RetrievalReference, CoreBankingRefNo = postingResult.Reference, DateCreated = DateTime.Now,
                        Fee = model.Fee, ResponseCode = postingResult.ResponseCode, ResponseMessage = postingResult.ResponseMessage, Status = postingResult.IsSuccessful.ToString(),
                        TransactionDate = DateTime.Now, TransactionType = "Credit"
                    };
                    context.Transactions.Add(dbRequest);
                    context.SaveChanges();
                }
                return ResponseDictionary.GetCodeDescription(postingResult.ResponseCode, postingResult.ResponseMessage, postingResult.Reference);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel DebitAccount(PostingModel model)
        {
            string method = "DebitAccount";
            LogMachine.LogInformation(Classname, method, $"entered the service for debiting account "+model.AccountNumber);
            try
            {                           
              
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}/CoreTransactions/Debit";
                var postingResult = apiservice.UrlPost<PostingResponse>(Url, model);
                if (postingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, debit failed");
                }
                if (postingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("06", " no response from server, debit failed");
                }
                if (postingResult.IsSuccessful==false)
                {
                    return ResponseDictionary.GetCodeDescription("06", postingResult.ResponseMessage);
                }
                using (AiroPayContext context = new AiroPayContext())
                {
                    var dbRequest = new Transactions()
                    {
                        AccountNo = model.AccountNumber,
                        Amount = model.Amount,
                        ApplicationRefNo = model.RetrievalReference,
                        CoreBankingRefNo = postingResult.Reference,
                        DateCreated = DateTime.Now,
                        Fee = model.Fee,
                        ResponseCode = postingResult.ResponseCode,
                        ResponseMessage = postingResult.ResponseMessage,
                        Status = postingResult.IsSuccessful.ToString(),
                        TransactionDate = DateTime.Now,
                        TransactionType = "Debit"
                    };
                    context.Transactions.Add(dbRequest);
                    context.SaveChanges();
                }
                return ResponseDictionary.GetCodeDescription(postingResult.ResponseCode, postingResult.ResponseMessage,postingResult.Reference);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06","System Error");
            }
        }

        public ReversalResponse Reversal(ReversalRequest request)
        {
            string method = "Reversal";
            LogMachine.LogInformation(Classname, method, $"entered the reversal service");
            try
            {
                var apiRequest = new ReversalApiRequest()
                {
                    Amount = request.Amount, 
                    RetrievalReference = request.RetrievalReference,
                    Token = BaseService.GetAppSetting("AuthToken"),
                    TransactionDate = request.TransactionDate, 
                    TransactionType = request.TransactionType
                      
                };
                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}CoreTransactions/Reversal";
                var postingResult = apiservice.UrlPost<ReversalResponse>(Url, apiRequest);
                if (postingResult == null)
                {
                    return new ReversalResponse { ResponseCode= "05", ResponseMessage= " no response from server, reversal failed" };
                }
               
                if (postingResult.IsSuccessful ==false)
                {
                    return new ReversalResponse { ResponseCode = "06", ResponseMessage = postingResult.ResponseMessage??"reversal failed" };                   
                }
                using (AiroPayContext context = new AiroPayContext())
                {
                    var dbRequest = new Reversal()
                    {
                        
                        Amount = request.Amount,                       
                        TransactionDate = DateTime.Now.ToString(),
                        TransactionType = "Reversal",
                         DateCreated=DateTime.Now,
                          RetrievalReference=request.RetrievalReference
                    };
                    context.Reversal.Add(dbRequest);
                    context.SaveChanges();
                }
                postingResult.ResponseCode = postingResult.ResponseCode ?? "00";
                postingResult.ResponseMessage = postingResult.ResponseMessage ?? "Successful";
                return postingResult;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return new ReversalResponse { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
        }

        public TransactionQueryResponse TransactionQuery(TransactionQueryRequest request)
        {
            string methodName = "TransactionQuery";
            try
            {
                var apiRequest = new TransactionQueryApiRequest()
                {
                    Amount = request.Amount,
                    RetrievalReference = request.RetrievalReference,
                    Token = BaseService.GetAppSetting("AuthToken"),
                    TransactionDate = request.TransactionDate,
                    TransactionType = request.TransactionType
                };

                string Url = $"{BaseService.GetAppSetting("ThirdPartyBankingBaseUrl")}CoreTransactions/TransactionStatusQuery ";
                var response = apiservice.UrlPost<TransactionQueryResponse>(Url, apiRequest);
                if (response == null)
                {
                    return new TransactionQueryResponse { ResponseCode = "05",  ResponseMessage =" no response from server"};
                }
                if (response.IsSuccessful == false)
                {
                    return new TransactionQueryResponse { ResponseCode = "04", ResponseMessage = response.ResponseMessage??"Request Failed" };
                   
                }
                return response;
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, Classname, methodName, ex);
                return new TransactionQueryResponse { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
        }
    }
}
