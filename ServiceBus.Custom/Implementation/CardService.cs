using ServiceBus.Core.Model.Generic;
using ServiceBus.Custom.Contract;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Implementation
{
    public class CardService : ICardService
    {
        string ClassName = "CardService";

        IBankOneCardIntegration crdService;
        public CardService(IBankOneCardIntegration service)
        {
            crdService = service;
        }

        public ResponseModel CardRequest(CardRequestModel request)
        {
            string method = "CardRequest";
            LogMachine.LogInformation(ClassName, method, $"entered the service for card reqeust {request.AccountNumber}");
            try
            {
                if (string.IsNullOrEmpty(request.AccountNumber))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Invalid Account Number");
                }
                if (string.IsNullOrEmpty(request.Token))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Invalid Token");
                }
                if (string.IsNullOrEmpty(request.BIN))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Invalid BIN");
                }
                if (string.IsNullOrEmpty(request.DeliveryOption))
                {
                    return ResponseDictionary.GetCodeDescription("04", "Delivery Option is required");
                }
                var result =crdService.CardRequest(request);
                using (AiroPayContext context=new AiroPayContext())
                {
                    var model = new CardRequest();
                    model.AccountNumber = request.AccountNumber;
                    model.BIN = request.BIN;
                    model.DeliveryOption = request.DeliveryOption;
                    model.Identifier = request.Identifier;
                    model.NameOnCard = request.NameOnCard;
                    model.RequestType = request.RequestType;
                    model.Status = result.ResponseDescription;
                    model.DateAdded = DateTime.Now;
                    context.CardRequest.Add(model);
                    context.SaveChanges();
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06","System Error");
            }
        }

        public ResponseModel GetCardDeliveryOptions()
        {
            string method = "GetCardDeliveryOptions";
            LogMachine.LogInformation(ClassName, method, $"entered the service for card delivery option");
            try
            {
                //call NIP service 
                return crdService.GetCardDeliveryOptions();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel GetCustomerCards(CustomerCardRequestModel request)
        {
           
            string method = "GetCustomerCards";
            LogMachine.LogInformation(ClassName, method, $"entered the service for GetCustomerCards");
            try
            {
                //call NIP service 
                return crdService.GetCustomerCards(request);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        
        }

        public ResponseModel HotlistCard(HotlistRequestModel request)
        {
            string method = "HotlistCard";
            LogMachine.LogInformation(ClassName, method, $"entered the service for HotlistCard");
            try
            {
                //call NIP service 
                return crdService.HotlistCard(request);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }

        public ResponseModel RetrieveInstitutionConfig()
        {
            string method = "RetrieveInstitutionConfig";
            LogMachine.LogInformation(ClassName, method, $"entered the service for card RetrieveInstitutionConfig");
            try
            {
                //call NIP service 
                return crdService.RetrieveInstitutionConfig();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "System Error");
            }
        }
    }
}
