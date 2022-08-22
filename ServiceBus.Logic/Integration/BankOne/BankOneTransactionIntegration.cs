using ServicBus.Logic.Contracts;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Settings;
using ServiceBus.Logic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBus.Core;
using Newtonsoft.Json;
using ServiceBus.Logic.Model.History;
using ServiceBus.Logic.Service;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Contracts.BankOne;

namespace ServiceBus.Logic.Integration.BankOne
{
    public class BankOneTransactionIntegration: IBankOneTransactionIntegration
    {

        string className = "AccountCreationService";
        IBankOneCoreBankingAuthentication coreBankingService;
        IApiPostAndGet apiservice;

        public BankOneTransactionIntegration(IBankOneCoreBankingAuthentication corebanking, IApiPostAndGet service)
        {
            coreBankingService = corebanking;
            apiservice = service;
        }

        public GetTransactionHistoryResponse GetTransaction(GetTransactionHistoryRequest request)
        {
            GetTransactionHistoryResponse response = new GetTransactionHistoryResponse();
            var historyResponse = new BankoneTransactionHistoryResponse();
            string methodName = "GetTransaction";
            try
            {
              

                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GetTransactions/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={request.AccountNumber}&fromDate={request.StartDate}&toDate={request.EndDate}&institutionCode={BaseService.GetAppSetting("BankOneCode")}&numberOfItems={request.NumberOfItems}";
                var stringResult = apiservice.UrlGet(Url, "");

                if (string.IsNullOrEmpty(stringResult))
                {
                    return new GetTransactionHistoryResponse() { ResponseCode = "01", ResponseMessage = " no response from server", OperatorId = request.OperatorId, };
                }

                try
                {
                    historyResponse = JsonConvert.DeserializeObject<BankoneTransactionHistoryResponse>(stringResult);
                }
                catch (Exception ex)
                {
                    //extract second response 
                   var secondBaseResponse = JsonConvert.DeserializeObject<TransactionBaseResponse>(stringResult);
                    return new GetTransactionHistoryResponse() { ResponseCode = "01", ResponseMessage = secondBaseResponse.Message??" no response from server", OperatorId = request.OperatorId, };
                }


                if (historyResponse.IsSuccessful == false)
                {
                    return new GetTransactionHistoryResponse() { ResponseCode = "01", ResponseMessage = "request failed", OperatorId = request.OperatorId, BankId=request.BankCode };
                }
                if (historyResponse.Message.Count <= 0)
                {
                    return new GetTransactionHistoryResponse() { ResponseCode = "01", ResponseMessage = "no record found for date range", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                var historyList = new List<TransactionHistory>();
                foreach (var item in historyResponse.Message)
                {
                    historyList.Add(new TransactionHistory()
                    {
                        CurrentDate = item.CurrentDate,
                        AccountNumber = item.AccountNumber,
                        Amount = item.Amount/100,
                        Balance = item.Balance/100,
                        Credit = item.Credit,
                        Debit = item.Debit,
                        InstrumentNo = item.InstrumentNo,
                        IsCardTransation = item.IsCardTransation,
                        IsReversed = item.IsReversed,
                        Narration = item.Narration,
                        OpeningBalance = item.OpeningBalance,
                        PostingType = item.PostingType,
                        ReferenceID = item.ReferenceID,
                        ReversalReferenceNo = item.ReversalReferenceNo,
                        ServiceCode = item.ServiceCode,
                        TransactionDate = item.TransactionDate,
                        TransactionDateString = item.TransactionDateString,
                        UniqueIdentifier = item.UniqueIdentifier,
                        WithdrawableAmount = item.WithdrawableAmount


                    }) ;
                }

                return new GetTransactionHistoryResponse()
                {
                    ResponseCode = "00",
                    ResponseMessage = "Request Successful",
                    AccountNumber = request.AccountNumber,
                    OperatorId = request.OperatorId,
                    TransactionHistory = historyList,
                    BankId = request.BankCode,
                    Page = historyResponse.Page,
                    TransactionTrackingRef = historyResponse.TransactionTrackingRef
                };
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    GetTransactionHistoryResponse()
                    { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }


        public GetMiniStatementResponse GetMiniStatement(GetMiniStatementRequest request)
        {
            GetMiniStatementResponse response = new GetMiniStatementResponse();
            var historyResponse = new BankoneTransactionHistoryResponse();
            string methodName = "GetMiniStatement";
            try
            {
                int NumberOfItems = 10;
                DateTime StartDate = DateTime.Now.AddDays(-30);
                DateTime EndDate = DateTime.Now;


                string Url = $"{AppConfig.CoreBankingBaseUrl}Account/GetTransactions/{BaseService.GetAppSetting("ApiVersion")}?authtoken={BaseService.GetAppSetting("AuthToken")}&accountNumber={request.AccountNumber}&fromDate={StartDate}&toDate={EndDate}&institutionCode={BaseService.GetAppSetting("BankOneCode")}&numberOfItems={NumberOfItems}";

                var stringResult = apiservice.UrlGet(Url, "");

                if (string.IsNullOrEmpty(stringResult))
                {
                    return new GetMiniStatementResponse() { ResponseCode = "01", ResponseMessage = " no response from server", OperatorId = request.OperatorId, };
                }

                try
                {
                    historyResponse = JsonConvert.DeserializeObject<BankoneTransactionHistoryResponse>(stringResult);
                }
                catch (Exception ex)
                {
                    //extract second response 
                    var secondBaseResponse = JsonConvert.DeserializeObject<TransactionBaseResponse>(stringResult);
                    return new GetMiniStatementResponse() { ResponseCode = "01", ResponseMessage = secondBaseResponse.Message ?? " no response from server", OperatorId = request.OperatorId, };
                }


                if (historyResponse.IsSuccessful == false)
                {
                    return new GetMiniStatementResponse() { ResponseCode = "01", ResponseMessage = "request failed", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
                if (historyResponse.Message.Count <= 0)
                {
                    return new GetMiniStatementResponse() { ResponseCode = "04", ResponseMessage = "no record found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                var historyList = new List<MiniHistory>();
                foreach (var item in historyResponse.Message)
                {
                    historyList.Add(new MiniHistory()
                    {
                       
                        Amount = item.Amount/100,
                        Balance = item.Balance/100,
                        Credit = item.Credit,
                        Debit = item.Debit,                        
                        IsReversed = item.IsReversed,
                        Narration = item.Narration,                      
                        PostingType = item.PostingType,
                        ReferenceID = item.ReferenceID,                        
                        TransactionDate = item.TransactionDate,                      
                        UniqueIdentifier = item.UniqueIdentifier
                    });
                }

                return new GetMiniStatementResponse()
                {
                    ResponseCode = "00",
                    ResponseMessage = "Request Successful",
                    AccountNumber = request.AccountNumber,
                    OperatorId = request.OperatorId,
                    MiniHistory = historyList,
                    BankId = request.BankCode
                    
                };
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return
                    new
                    GetMiniStatementResponse()
                    { ResponseCode = "06", ResponseMessage = "System Malfunction, Kindly retry or contact admin", OperatorId = request.OperatorId, BankId = request.BankCode };
            }
        }
    }
}
