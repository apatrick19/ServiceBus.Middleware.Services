using ServiceBus.Logic.Contracts;
using System;
using System.Diagnostics;
using ServiceBus.Logic.Model;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Core;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Logic.Model.Transfer;
using ServiceBus.Logic.Service;
using ServiceBus.Core.DataTransferObject;

namespace ServiceBus.Logic.Integration
{
    public class TransferLogicService : ITransferLogicService
    {

        string className = "TransferLogicService";

        IAccountValidationService validationService;
        IPostingIntegrationService postingService;
        ITransferIntegration transferIntegration;
        public TransferLogicService(IAccountValidationService service, IPostingIntegrationService postService, ITransferIntegration nIPBaseService)
        {
            validationService = service;
            postingService = postService;
            transferIntegration = nIPBaseService;
        }

        public FundTransferResponse InterBank(FundsTransferRequest request)
        {
            string methodName = "InterBank";
            try
            {
                string refrenceNo = DateTime.Now.ToString("yyMMddHHmmss");

                if (string.IsNullOrEmpty(request.PayerAccountNumber))
                {
                    return new FundTransferResponse() { ResponseCode= "01", ResponseMessage= "Invalid account No" };
                }
                if (string.IsNullOrEmpty(request.ReceiverAccountNumber))
                {
                    return new FundTransferResponse() { ResponseCode = "01", ResponseMessage = "Invalid Receiver Account Number" };
                }

                using (AiroPayContext context = new AiroPayContext())
                {

                    var apiRequest = new FundTransferApiRequest()
                    {
                        Amount = request.Amount * 100,
                        AppzoneAccount = "",                        
                        IsBeneficiaryTransfer = false,
                        Narration = request.Narration,
                        NIPSessionID = request.NIPSessionID, 
                        Payer = request.Payer,
                        PayerAccountNumber = request.PayerAccountNumber,
                        ReceiverAccountNumber = request.ReceiverAccountNumber, 
                        PIN = string.Empty,
                        ReceiverAccountType = "00",
                        ReceiverBankCode = request.ReceiverBankCode,
                        ReceiverBVN = "",
                        ReceiverKYC = "",
                        ReceiverName = request.ReceiverName,
                        ReceiverPhoneNumber = "08000000000",
                        Token = BaseService.GetAppSetting("AuthToken"),
                        TransactionReference = request.TransactionReference
                         
                    };
                  

                    var response = transferIntegration.FundTransfer(apiRequest);
                    var dbModel = new Transfer()
                    {
                        Amount = request.Amount,
                        BeneficiaryAccountNumber = request.ReceiverAccountNumber,
                        DestinationBankCode = request.ReceiverBankCode,
                        Narration = request.Narration,
                        DateCreated = DateTime.Now,
                        PaymentReference = refrenceNo,
                        SourceAccountNo = request.PayerAccountNumber,
                        SessionId = request.NIPSessionID,
                        SourceBankCode = BaseService.GetAppSetting("BankOneCode"),
                        TransferType = "InterBankTransfer",
                        ResponseCode = response?.ResponseCode,
                        ResponseMessage = response?.ResponseMessage,
                        BeneficiaryAccountName = request.ReceiverName,
                        BeneficiaryBVN = "",
                        Kyc = "",
                        NameEnquiryReference = request.NIPSessionID,
                        SourceAccountName = request.Payer
                            
                    };
                    context.Transfer.Add(dbModel);
                    context.SaveChanges();
                    return response;
                }

            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return new FundTransferResponse() { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
        }

        public LocalTransferResponse IntraBank(LocalFundTransferRequest request)
        {
            string methodName = "IntraBank";
            try
            {
               // string refrenceNo = DateTime.Now.ToString("yyMMddHHmmss");
               
                if (string.IsNullOrEmpty(request.FromAccountNumber))
                {
                    return new LocalTransferResponse() { ResponseCode = "01", ResponseMessage = "Invalid Sender Account Number" };
                }
                if (string.IsNullOrEmpty(request.ToAccountNumber))
                {
                    return new LocalTransferResponse() { ResponseCode = "01", ResponseMessage = "Invalid Reciever Account Number" };
                }
               
                using (AiroPayContext context=new AiroPayContext())
                {
                    var apiRequest = new LocalTransferApiRequest()
                    {
                        Amount = request.Amount * 100,
                        AuthenticationKey = BaseService.GetAppSetting("AuthToken"),
                        Fee = 0,
                        FromAccountNumber = request.FromAccountNumber,
                        Narration = request.Narration,
                        RetrievalReference = request.RetrievalReference,
                        ToAccountNumber = request.ToAccountNumber                       
                    };
                    var response= transferIntegration.LocalTransfer(apiRequest);
                    var dbModel = new Transfer()
                    {
                        Amount = apiRequest.Amount,
                        BeneficiaryAccountNumber = request.ToAccountNumber,
                        DestinationBankCode = BaseService.GetAppSetting("BankOneCode"),
                        Narration = request.Narration,
                        DateCreated = DateTime.Now,
                        PaymentReference = request.RetrievalReference,
                        SourceAccountNo = apiRequest.FromAccountNumber,
                        SessionId = apiRequest.RetrievalReference,
                        SourceBankCode = BaseService.GetAppSetting("BankOneCode"),
                        TransferType = "LocalTransfer",
                        ResponseCode = response?.ResponseCode,
                        ResponseMessage=response?.ResponseMessage
                    };
                    context.Transfer.Add(dbModel);
                    context.SaveChanges();
                    return response;
                }
               
            }
            catch (Exception ex)
            {              
                LogService.LogError(request.OperatorId, className, methodName, ex);
                return new LocalTransferResponse() { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
}

        public NameEnquiryResponse NameEnquiry(NameEnquiryRequest request)
        {
            try
            {
                if (request.AccountNumber.Length > 10)
                {
                 
                    return new NameEnquiryResponse() { ResponseCode = "01", ResponseMessage = "Invalid account No; account no exceeds 10 digit " };
                }

                if (string.IsNullOrEmpty(request.BankCode))
                {
                    return new NameEnquiryResponse() { ResponseCode = "04", ResponseMessage = "Invalid Bank Code" };
                }
              
              

                return transferIntegration.NameEnquiry(request);

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return new NameEnquiryResponse() { ResponseCode = "06", ResponseMessage = "System Malfuntion" };
            }
        }

       
    }
}
