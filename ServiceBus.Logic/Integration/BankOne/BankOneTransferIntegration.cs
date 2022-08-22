using ServiceBus.Logic.Model;
using ServiceBus.Logic.Contracts;
using Newtonsoft.Json;
using ServicBus.Logic.Contracts;
using ServiceBus.Core.Settings;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBus.Logic;
using ServiceBus.Core;
using ServiceBus.Logic.Model.Transfer;
using ServiceBus.Core.DataTransferObject;

namespace manny.ussd.logic.Integration
{
    public class BankOneTransferIntegration: ITransferIntegration
    {
        static string Classname = "TransferIntegration";
        IApiPostAndGet apiservice;
        public BankOneTransferIntegration(IApiPostAndGet apiPostAndGet)
        {
            apiservice = apiPostAndGet;
        }
        public NameEnquiryResponse NameEnquiry(NameEnquiryRequest Request)
        {
            string method = "NameEnquiry";
            var response = new ResponseModel();
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
              var apiRequest=  new NameEnquiryApiRequest() { AccountNumber=Request.AccountNumber, Token=BaseService.GetAppSetting("AuthToken"), BankCode=Request.CommBankCode  };
                             
                string Url = string.Concat(BaseService.GetAppSetting("ThirdPartyBankingBaseUrl"), "Transfer/NameEnquiry");
               
                var  result = apiservice.UrlPost<NameEnquiryResponse>(Url, apiRequest);
                if (result==null)
                {
                    return new NameEnquiryResponse() { ResponseCode= "06", ResponseMessage="no response from server" };
                }
                if (result.IsSuccessful==false)
                {
                    return new NameEnquiryResponse() { ResponseCode = "06", ResponseMessage = result.ResponseMessage?? "validation failed" };                   
                }
                result.ResponseCode = result.ResponseCode??"00";
                result.ResponseMessage = result.ResponseMessage ?? "Successful";
                return result;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                response.ResponseCode = "06";
                response.ResponseDescription = "application error, could not get name";
                return new NameEnquiryResponse() { ResponseCode = "06", ResponseMessage = "System Malfuntion" };
            }
        }

        public FundTransferResponse FundTransfer(FundTransferApiRequest Request)
        {
            string method = "FundTransfer";
            var response = new FundTransferResponse();
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {               
                string Url = string.Concat(BaseService.GetAppSetting("ThirdPartyBankingBaseUrl"), "Transfer/InterbankTransfer");

                var result = apiservice.UrlPost<FundTransferApiResponse>(Url, Request);
                if (result==null)
                {
                    return new FundTransferResponse() { ResponseCode = "05", ResponseMessage = "no response from server" };
                }
                if (result.IsSuccessFul == false)
                {
                    return new FundTransferResponse() { ResponseCode="06",  ResponseMessage=result.ResponseMessage??"Tranfer Failed" };
                }
                return  new FundTransferResponse() { ResponseCode = result.ResponseCode??"00", ResponseMessage = result.ResponseMessage??"Successful", Reference=result.Reference };
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                response.ResponseCode = "96";
                response.ResponseMessage = "System Malfunction ";
                return response;
            }
        }

        public LocalTransferResponse LocalTransfer(LocalTransferApiRequest Request)
        {
            string method = "LocalTransfer";
          
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                string Url = string.Concat(BaseService.GetAppSetting("ThirdPartyBankingBaseUrl"), "CoreTransactions/LocalFundsTransfer");
                var  response = apiservice.UrlPost<LocalTransferResponse>(Url, Request);
                if (response==null)
                {
                    return new LocalTransferResponse() { ResponseCode = "05", ResponseMessage = "no response from server" };
                }
                if (response.ResponseCode!="00")
                {
                    return new LocalTransferResponse() { ResponseCode = "06", ResponseMessage = response.ResponseMessage ?? "Transfer Failed"};
                }
                response.ResponseCode = response.ResponseCode ?? "00";
                response.ResponseMessage = response.ResponseMessage ?? "Successful";
                return response;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return new LocalTransferResponse() { ResponseCode="96", ResponseMessage="System Malfunction" };
            }
        }
    }
}
