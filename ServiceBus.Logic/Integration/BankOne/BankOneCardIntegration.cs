using ServicBus.Logic.Contracts;
using ServiceBus.Core;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration
{
    public class BankOneCardIntegration : IBankOneCardIntegration
    {

        string Classname = "CardIntegration";

        IApiPostAndGet apiservice;
        public BankOneCardIntegration(IApiPostAndGet apiPostAndGet)
        {
            apiservice = apiPostAndGet;
        }
        public ResponseModel CardRequest(CardRequestModel request)
        {
            string method = "CardRequest";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/RequestCard";
                var billingResult = apiservice.UrlPost<CardResponseModel>(Url, request);
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", billingResult.ResponseMessage);
                }
                return ResponseDictionary.GetCodeDescription("00", billingResult.ResponseMessage, billingResult.Identifier);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetCardDeliveryOptions()
        {
            string method = "GetCardDeliveryOptions";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/GetCardDeliveryOptions/"+ BaseService.GetAppSetting("AuthToken");
                var billingResult = apiservice.UrlGet<DeliveryOptionReponse>(Url, "");
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", billingResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", billingResult.ResponseDescription, billingResult.DeliveryOptions);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel GetCustomerCards(CustomerCardRequestModel request)
        {
            string method = "GetCustomerCards";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/RetrieveCustomerCards";
                var billingResult = apiservice.UrlPost<CustomerCardResponseModel>(Url, request);
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", billingResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", billingResult.ResponseDescription, billingResult.Cards);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel HotlistCard(HotlistRequestModel request)
        {
            string method = "HotlistCard";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/HotlistCard";
                var billingResult = apiservice.UrlPost<HotlistResponseModel>(Url, request);
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", billingResult.ResponseMessage);
                }
                return ResponseDictionary.GetCodeDescription("00", billingResult.ResponseMessage, billingResult.SerialNo);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel RetrieveInstitutionConfig()
        {
            string method = "RetrieveInstitutionConfig";
            LogMachine.LogInformation(Classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/RetrieveInstitutionConfig/" + BaseService.GetAppSetting("AuthToken");
                var billingResult = apiservice.UrlGet<CardConfigurationResponse>(Url,"");
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", billingResult.ResponseDescription);
                }
                return ResponseDictionary.GetCodeDescription("00", billingResult.ResponseDescription, billingResult);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(Classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }
    }
}
