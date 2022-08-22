using ServiceBus.Core.Model.Bank;
using ServiceBus.Custom.Contract;
using ServiceBus.Custom.Model;
using ServiceBus.Data.ORM.EntityFramework;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBus.Core.ControllerModel;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Model;
using ServiceBus.Core.Settings;
using ServicBus.Logic.Implementations.Memory;
using ServicBus.Logic.Implementations.Security;
using ServiceBus.Logic.Model.Quickteller;
using ServiceBus.Logic.Model.Quickteller.bills;
using ServiceBus.Logic.Model.PaymentItemSpace;
using manny.ussd.logic.Model;
using ServiceBus.Core.Model.Generic;

namespace ServiceBus.Custom.Implementation
{
    public class BillingService : IBillingService
    {
        IPostingIntegrationService postingService;
        IBillerLogicService billerService;
        IAccountValidationService validationService;
        public BillingService(IPostingIntegrationService postingServiceParams, IBillerLogicService billerParams, IAccountValidationService service)
        {
            postingService = postingServiceParams;
            billerService = billerParams;
            validationService = service;
        }

      
        #region cabletv
      

        public ResponseModel CustomerValidation(CustomerValidation request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CustomerId))
                {
                    return ResponseDictionary.GetCodeDescription("06", "invalid customer id");
                }
                if (string.IsNullOrEmpty(request.ProviderCode))
                {
                    return ResponseDictionary.GetCodeDescription("06", "invalid provider code");
                }
                string guid = Guid.NewGuid().ToString();
                var apiRequest = new CustomerApiRequest()
                {
                    CountryCode = "NG",
                    CustomerId = request.CustomerId,
                    Email = "",
                    MobileNo = request.MobileNo,
                    PaymentCode = request.ProviderCode,
                    ReferenceNo = guid,
                    RequestId = guid
                };
                return billerService.ValidateCustomer(apiRequest);


            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
       
        #endregion
        public ResponseModel GetBillingCategory()
        {
            try
            {                
                var result = billerService.GetCategory();
                if (result.ResponseCode == "00")
                {
                    List<BaseCategory> products = new List<BaseCategory>();
                    var objects = (List<CategoryResponse>)result.ResultObject;
                    foreach (var item in objects)
                    {
                        products.Add(new BaseCategory()
                        {
                            CategoryDescription = item.Description,
                            CategoryId = item.ID,
                            CategoryName = item.Name
                        });
                    }
                    return ResponseDictionary.GetCodeDescription("00", products);
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetBillingMerchants(string CategorId)
        {
            try
            {
                List<BaseBiller> products = new List<BaseBiller>();
                using (AiroPayContext context=new AiroPayContext())
                {
                    if (string.IsNullOrEmpty(CategorId))
                    {
                        return ResponseDictionary.GetCodeDescription("04", "invalid category code");
                    }
                    var dbResult = context.Billers.Where(x => x.CategoryId == CategorId).ToList();
                    if (dbResult.Count<=0)
                    {
                        var billerResult = billerService.GetBillers();
                        if (billerResult.ResponseCode=="00")
                        {
                            var objects = (List<BillersResponse>)billerResult.ResultObject;
                            foreach (var item in objects)
                            {
                                if (item.CategoryId==CategorId)
                                {
                                    products.Add(new BaseBiller()
                                    {
                                        BillerId = item.ID,
                                        BillerName = item.Name
                                    });
                                }
                                
                            }
                            return ResponseDictionary.GetCodeDescription("00","success", products.OrderBy(x => x.BillerName));
                        }
                        return ResponseDictionary.GetCodeDescription("04", "no record found");
                    }
                    else
                    {                    
                        foreach (var item in dbResult)
                        {
                            products.Add(new BaseBiller()
                            {
                                BillerId = item.ID,
                                BillerName = item.Name
                            });
                        }
                        return ResponseDictionary.GetCodeDescription("00","Success", products.OrderBy(x => x.BillerName));
                    }
                  
                }
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }
               
        public ResponseModel SendPayment(BillsPaymentRequest request)
        {
            try
            {
                var trxLog = new BillsPaymentTransaction();
                if (request.Amount <= 0)
                {
                    return ResponseDictionary.GetCodeDescription("06", "invalid amount");
                }
                if (string.IsNullOrEmpty(request.AccountNumber))
                {
                    return ResponseDictionary.GetCodeDescription("06", "please provide account no");
                }             
              
                using (AiroPayContext context = new AiroPayContext())
                {
                    var mobileAcct = context.Account.Where(x => x.AccountNumber == request.AccountNumber).FirstOrDefault();
                    if (mobileAcct != null)
                    {
                        if (mobileAcct.PIN != request.PIN)
                        {
                            return ResponseDictionary.GetCodeDescription("06", "Invalid PIN");
                        }
                    }
                    else
                    {
                        return ResponseDictionary.GetCodeDescription("04", "Account not registered for ussd service");
                    }
                  
                    var billerResponse= billerService.SendPayment(request);
                    trxLog.Amount = request.Amount;
                    trxLog.TransactionDate = DateTime.Now;
                    trxLog.TransactionType = request.BillerName;                   
                    trxLog.ThirdPartyResponseCode = billerResponse.ResponseCode;
                    trxLog.ThirdPartyResponseMessage = billerResponse.ResponseDescription;     
                    trxLog.isSuccessful = billerResponse.ResponseCode == "00" ? true : false;
                    context.BillsPaymentTransaction.Add(trxLog);
                    context.SaveChanges();
                    return billerResponse;
                }

            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06");
            }
        }

        public ResponseModel GetPaymentItems(string billerid)
        {
            try
            {
                if (string.IsNullOrEmpty(billerid))
                {
                    return ResponseDictionary.GetCodeDescription("04", "invalid provider");
                }
                //call integration
                var result = billerService.GetPaymentItems();
                if (result.ResponseCode == "00")
                {
                    List<BaseAirtimeProduct> products = new List<BaseAirtimeProduct>();
                    var objects = (List<PaymentItemsResponse>)result.ResultObject;
                    foreach (var item in objects)
                    {
                        products.Add(new BaseAirtimeProduct()
                        {
                            Amount = item.Amount,
                            Name = item.Name,
                            PaymentCode = item.ID
                        });
                    }
                    return ResponseDictionary.GetCodeDescription("00", products);
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation($"An error occurred see stack {ex?.Message}; {ex?.InnerException} {ex?.InnerException?.StackTrace}");
                return ResponseDictionary.GetCodeDescription("06", "could not retrieve products");
            }
        }
    }
}
