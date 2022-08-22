using ServicBus.Logic.Implementations.Memory;
using ServiceBus.Core.DataTransferObject;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Custom.Contract;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.PortalModel;
using ServiceBus.Logic.Model.Transfer;
using ServiceBus.Logic.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Implementation
{
    public class TransferBaseService : ITransferBaseService
    {
        string ClassName = "TransferBaseService";
        ITransferLogicService trfService;
        public TransferBaseService(ITransferLogicService transfer)
        {
            trfService = transfer;
        }

        public NameEnquiryResponse GetBeneficiaryName(NameEnquiryRequest request)
        {
            string method = "GetBeneficiaryName";
            LogMachine.LogInformation(ClassName, method, $"entered the service for name enquiry {request.AccountNumber}");
            try
            {              

                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         NameEnquiryResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return trfService.NameEnquiry(request);
                    case Core.enums.OperatorEnum.Others:
                        return new NameEnquiryResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new NameEnquiryResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new NameEnquiryResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return new NameEnquiryResponse() { ResponseCode = "06", ResponseMessage = "System Malfuntion" };
            }
        }

        public LocalTransferResponse IntraBank(LocalFundTransferRequest request)
        {
            string methodName = "IntraBank";
            try
            {               

                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         LocalTransferResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return trfService.IntraBank(request);
                    case Core.enums.OperatorEnum.Others:
                        return new LocalTransferResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new LocalTransferResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new LocalTransferResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }
                                
             }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, ClassName, methodName, ex);
                return new LocalTransferResponse() { ResponseCode = "06", ResponseMessage = "System Malfuntion" }; ;
            }
        }
        
        public FundTransferResponse InterBank(FundsTransferRequest request)
        {
            string methodName = "InterBank";
            try
            {
                if (string.IsNullOrEmpty(request.ReceiverBankCode))
                {
                    return new FundTransferResponse() { ResponseCode = "01", ResponseMessage = "Invalid destination bank selected" };                    
                }
                if (string.IsNullOrEmpty(request.PayerAccountNumber))
                {
                    return new FundTransferResponse() { ResponseCode = "01", ResponseMessage = "Invalid account No" };                 
                }
                if (string.IsNullOrEmpty(request.ReceiverAccountNumber))
                {
                    return  new FundTransferResponse() { ResponseCode = "01", ResponseMessage = "Invalid Receiver Account Number" };
                }
                

                if (OperatorService.ValidateOperator(request.OperatorId).ResponseCode != "00")
                {
                    return
                         new
                         FundTransferResponse()
                         { ResponseCode = "02", ResponseMessage = "Invalid Operator Id", OperatorId = request.OperatorId, BankId = request.BankCode };
                }

                switch (OperatorService.GetOperator(request.OperatorId))
                {
                    case Core.enums.OperatorEnum.BankOne:
                        return trfService.InterBank(request);
                    case Core.enums.OperatorEnum.Others:
                        return new FundTransferResponse() { ResponseCode = "03", ResponseMessage = "Implementation in progress for this operator", OperatorId = request.OperatorId, BankId = request.BankCode };
                    case Core.enums.OperatorEnum.None:
                        return new FundTransferResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                    default:
                        return new FundTransferResponse() { ResponseCode = "03", ResponseMessage = "No Operator found", OperatorId = request.OperatorId, BankId = request.BankCode };
                }               
                
            }
            catch (Exception ex)
            {
                LogService.LogError(request.OperatorId, ClassName, methodName, ex);               
                return new FundTransferResponse() { ResponseCode = "96", ResponseMessage = "System Malfunction" };
            }
        }

        public ResponseModel AddBeneficiary(BeneficiaryModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.InitiatorAccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("04", "initiator account is invalid");
                }
                if (string.IsNullOrEmpty(model.BeneficiaryBankCode))
                {
                    return ResponseDictionary.GetCodeDescription("04", "please select beneficiary bank");
                }
                if (string.IsNullOrEmpty(model.BeneficiaryAccountNumber))
                {
                    return ResponseDictionary.GetCodeDescription("04", "please input beneficiary account number");
                }
                if (string.IsNullOrEmpty(model.BeneficiaryAccountName))
                {
                    return ResponseDictionary.GetCodeDescription("04", "please validate beneficiary account name");
                }
                //validate customers 
                if (model.InitiatorAccountNo.Length<10)
                {
                    return ResponseDictionary.GetCodeDescription("04", "invalid initiator account number");
                }
                if (model.BeneficiaryAccountNumber.Length < 10)
                {
                    return ResponseDictionary.GetCodeDescription("04", "invalid beneficiary account number");
                }
                if (model.BeneficiaryBankCode.Length < 6 && model.isMannyAccount==false)
                {
                    return ResponseDictionary.GetCodeDescription("04", "invalid bank code");
                }
                if (!Regex.IsMatch(model.InitiatorAccountNo, "\\A[0-9]{10}\\z"))
                {
                    return ResponseDictionary.GetCodeDescription("04", "invalid initiator account number");
                }
                if (!Regex.IsMatch(model.BeneficiaryAccountNumber, "\\A[0-9]{10}\\z"))
                {
                    return ResponseDictionary.GetCodeDescription("04", "invalid beneficiary account number");
                }


                using (AiroPayContext context = new AiroPayContext())
                {
                    var mobileAcct = context.Beneficiary.Where(x => x.InitiatorAccountNo == model.InitiatorAccountNo && x.BeneficiaryAccountNumber==model.BeneficiaryAccountNumber && x.BeneficiaryBankCode==model.BeneficiaryBankCode && x.isMannyAccount==model.isMannyAccount).FirstOrDefault();
                    if (mobileAcct != null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "beneficiary exists");
                    }
                    //create model
                    var dbRequest = new Beneficiary()
                    {
                        InitiatorAccountNo = model.InitiatorAccountNo,
                        BeneficiaryAccountName = model.BeneficiaryAccountName,
                        BeneficiaryAccountNumber = model.BeneficiaryAccountNumber,
                        BeneficiaryBankCode = model.BeneficiaryBankCode,
                        isMannyAccount = model.isMannyAccount,
                        BeneficiaryBankName=model.BeneficiaryBankName,
                        SessionId = ""
                    };
                    context.Beneficiary.Add(dbRequest);
                    context.SaveChanges();
                    //send SMS with hangfire 
                    return ResponseDictionary.GetCodeDescription("00","Beneficiary Added Succesfully");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetBeneficiary(string InitiatorAccountNo, bool IsMannyBeneficiary)
        {
            try
            {
                if (string.IsNullOrEmpty(InitiatorAccountNo))
                {
                    return ResponseDictionary.GetCodeDescription("04", "initiator account is invalid");
                }              
               
                using (AiroPayContext context = new AiroPayContext())
                {
                    var Beneficiary = context.Beneficiary.Where(x => x.InitiatorAccountNo == InitiatorAccountNo && x.isMannyAccount == IsMannyBeneficiary);
                    if (Beneficiary.Count()<=0)
                    {
                        return ResponseDictionary.GetCodeDescription("04", "no record found");
                    }                    
                    //send SMS with hangfire 
                    return ResponseDictionary.GetCodeDescription("00", Beneficiary.ToList());
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
    }
}
