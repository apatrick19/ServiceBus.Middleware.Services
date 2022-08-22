using ServicBus.Logic.Implementations;
using ServiceBus.Core;
using ServiceBus.Core.Model.Bank;
using ServiceBus.Core.Model.Generic;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Integration.Portal
{
   public  class CardsLogic
    {
       static string classname = "GetCardDeliveryOptions";
       

       
        public static CardConfigurationResponse FetchCardConfiguration()
        {
            string methodname = "FetchProducts";           
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/RetrieveInstitutionConfig/" + BaseService.GetAppSetting("AuthToken");
                var prdResult = new ApiPostAndGet().UrlGet<CardConfigurationResponse>(Url, "");
               
                return prdResult;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation("", classname, methodname, ex);
                return new CardConfigurationResponse() { IsSuccessful = false, ResponseDescription="System Error" };
            }
        }

        public static ResponseModel CardRequest(CardRequestModel request)
        {
            string method = "CardRequest";
            LogMachine.LogInformation(classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/RequestCard";
                var billingResult = new ApiPostAndGet().UrlPost<CardResponseModel>(Url, request);
                if (billingResult == null)
                {
                    return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                }
                if (billingResult.IsSuccessful == false)
                {
                    return ResponseDictionary.GetCodeDescription("06", billingResult.ResponseMessage);
                }
                using (AiroPayContext context = new AiroPayContext())
                {
                    var model = new CardRequest();
                    model.AccountNumber = request.AccountNumber;
                    model.BIN = request.BIN;
                    model.DeliveryOption = request.DeliveryOption;
                    model.Identifier = request.Identifier;
                    model.NameOnCard = request.NameOnCard;
                    model.RequestType = request.RequestType;
                    model.Status = billingResult.ResponseMessage;
                    model.DateAdded = DateTime.Now;
                    context.CardRequest.Add(model);
                    context.SaveChanges();
                }
                return ResponseDictionary.GetCodeDescription("00", billingResult.ResponseMessage, billingResult.Identifier);
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public static CustomerCardResponseModel GetCustomerCards(CustomerCardRequestModel request)
        {
            string method = "GetCustomerCards";
            LogMachine.LogInformation(classname, method, $"entered the service");
            try
            {
                string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/RetrieveCustomerCards";
                var billingResult = new ApiPostAndGet().UrlPost<CustomerCardResponseModel>(Url, request);
                if (billingResult == null)
                {
                    return new CustomerCardResponseModel() { IsSuccessful=false, ResponseDescription= " no response from server, request failed" };
                }
                if (billingResult.IsSuccessful == false)
                {
                    return billingResult;
                }
                return billingResult;
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, method, $"an error occurred {ex}");
                return new CustomerCardResponseModel() { IsSuccessful = false, ResponseDescription = " System Error" };
            }
        }

        public static ResponseModel HotlistCard(HotlistRequestModel request)
        {
            string method = "HotlistCard";
            LogMachine.LogInformation(classname, method, $"entered the service");
            try
            {
                using (AiroPayContext context=new AiroPayContext())
                {
                    string Url = BaseService.GetAppSetting("ThirdPartyBankingBaseUrl") + "Cards/HotlistCard";
                    var billingResult = new ApiPostAndGet().UrlPost<HotlistResponseModel>(Url, request);
                    if (billingResult == null)
                    {
                        return ResponseDictionary.GetCodeDescription("04", " no response from server, request failed");
                    }
                    if (billingResult.IsSuccessful == false)
                    {
                        return ResponseDictionary.GetCodeDescription("06", billingResult.ResponseMessage);
                    }

                    var dbLog = new CardHotListLog();
                    dbLog.AccountNumber = request.AccountNumber;
                    dbLog.SerialNo = request.SerialNo;
                    dbLog.Reference = request.Reference;
                    dbLog.Token = request.Token;
                    dbLog.HotlistReason = request.HotlistReason;
                    dbLog.DateCreated = DateTime.Now;
                    dbLog.Status = billingResult.ResponseMessage;
                    context.CardHotListLog.Add(dbLog);
                    context.SaveChanges();
                    return ResponseDictionary.GetCodeDescription("00", billingResult.ResponseMessage, billingResult.SerialNo);
                }
              
            }
            catch (Exception ex)
            {
                LogMachine.LogInformation(classname, method, $"an error occurred {ex}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }
    }
}
