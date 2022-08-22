using ServiceBus.Core.ControllerModel;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Contract
{
   public interface IBillingService
    {
      
        ResponseModel GetBillingCategory();

        ResponseModel GetBillingMerchants(string CategorId);
        ResponseModel GetPaymentItems(string billerid);         
      
        ResponseModel SendPayment(BillsPaymentRequest request);    
        ResponseModel CustomerValidation(CustomerValidation request);
      
    }
}
