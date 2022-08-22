using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts
{
    public interface IBankOneCardIntegration
    {
        ResponseModel GetCardDeliveryOptions();
        ResponseModel RetrieveInstitutionConfig();
        ResponseModel CardRequest(CardRequestModel request);
        ResponseModel GetCustomerCards(CustomerCardRequestModel request);
        ResponseModel HotlistCard(HotlistRequestModel request);


    }

}
