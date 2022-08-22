using ServiceBus.Core.DataTransferObject;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Contracts
{
   public interface ITransferLogicService
    {
        NameEnquiryResponse NameEnquiry(NameEnquiryRequest request);

        FundTransferResponse InterBank(FundsTransferRequest request);

        LocalTransferResponse IntraBank(LocalFundTransferRequest request);
    }
}
