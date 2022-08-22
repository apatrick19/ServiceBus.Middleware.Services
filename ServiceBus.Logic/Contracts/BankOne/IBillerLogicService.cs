using ServiceBus.Logic.Model;
using ServiceBus.Core.ControllerModel;
using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts
{
  public  interface IBillerLogicService
    {
        ResponseModel GetCategory();
        ResponseModel GetPaymentItems();
        ResponseModel GetBillers();             
        ResponseModel ValidateCustomer(CustomerApiRequest Request);
        ResponseModel SendPayment(BillsPaymentRequest Request);
    }
}
