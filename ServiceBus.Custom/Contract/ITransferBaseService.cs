using ServiceBus.Core.DataTransferObject;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Model;
using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Contract
{
   public interface ITransferBaseService
    {
        NameEnquiryResponse GetBeneficiaryName(NameEnquiryRequest request);

        LocalTransferResponse IntraBank(LocalFundTransferRequest request);
        FundTransferResponse InterBank(FundsTransferRequest request);
        ResponseModel AddBeneficiary(BeneficiaryModel model);
        ResponseModel GetBeneficiary(string InitiatorAccountNo, bool IsMannyBeneficiary);
    }
}
