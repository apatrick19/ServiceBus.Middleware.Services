using ServiceBus.Logic.Model;
using Newtonsoft.Json;
using ServicBus.Logic.Contracts;
using ServiceBus.Core.Settings;
using ServiceBus.Logic.Implementations;
using ServiceBus.Logic.Implementations.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBus.Logic.Model.Transfer;
using ServiceBus.Core.DataTransferObject;

namespace ServiceBus.Logic
{
  public  interface ITransferIntegration
    {
        NameEnquiryResponse NameEnquiry(NameEnquiryRequest Request);

        FundTransferResponse FundTransfer(FundTransferApiRequest Request);

        LocalTransferResponse LocalTransfer(LocalTransferApiRequest Request);
    }
}
