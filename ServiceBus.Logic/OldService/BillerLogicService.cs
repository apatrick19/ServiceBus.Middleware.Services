using ServiceBus.Logic.Model;
using Newtonsoft.Json;
using ServicBus.Logic.Contracts;
using ServiceBus.Core.ControllerModel;
using ServiceBus.Core.Settings;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Model.Quickteller;
using ServiceBus.Logic.Model.Quickteller.bills;
using ServiceBus.Logic.Model.PaymentItemSpace;
using ServiceBus.Core;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Core.Model.Generic;

namespace ServiceBus.Logic.Integration
{
   public  class BillerLogicService:IBillerLogicService
    {
        static string Classname = "BillerLogicService";
        IApiPostAndGet apiservice;
        public BillerLogicService(IApiPostAndGet apiPostAndGet)
        {
            apiservice = apiPostAndGet;
        }

        public ResponseModel GetCategory()
        {
            string method = "GetCategory";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "BillsPayment/GetBillerCategories/"+BaseService.GetAppSetting("AuthToken");
                var billingResult = apiservice.UrlGet<List<CategoryResponse>>(Url, "");
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.Count<=0)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                return ResponseDictionary.GetCodeDescription("00","Successful", billingResult);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetPaymentItems()
        {
            string method = "GetPaymentItems";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {

             
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "BillsPayment/GetPaymentItems/" + BaseService.GetAppSetting("AuthToken");               
                var billingResult = apiservice.UrlGet<List<PaymentItemsResponse>>(Url, "");
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.Count <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }
                return ResponseDictionary.GetCodeDescription("00", "Successful", billingResult);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetBillers()
        {
            string method = "GetBillers";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "BillsPayment/GetBillers/" + BaseService.GetAppSetting("AuthToken");
                var billingResult = apiservice.UrlGet<List<BillersResponse>>(Url, "");
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.Count <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no record found");
                }               
                return ResponseDictionary.GetCodeDescription("00", "Successful", billingResult);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }       

        public ResponseModel ValidateCustomer(CustomerApiRequest Request)
        {
            string method = "ValidateCustomer";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                var billsParams = JsonConvert.SerializeObject(Request);
                string Url = AppConfig.QuicktellerBaseUrl + "billers/validatecustomer";
                var billingResult = apiservice.UrlPostWithRestSharp<ResponseModel>(Url, billsParams, $"Basic UXVpY2t0ZWxsZXI6TEBnMHMxMjMkJV4=");
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, airtime failed");
                }

                return billingResult;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
          
        public ResponseModel SendPayment(BillsPaymentRequest Request)
        {
             decimal amount= Request.Amount * 100;
            Request.Amount = amount;
            string method = "SendPayment";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {

                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "BillsPayment/Payment";
                var billingResult = apiservice.UrlPost<BillsPaymentResponse>(Url, Request);
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", billingResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription(billingResult.ResponseCode,billingResult.ResponseMessage,billingResult);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
    }
}
