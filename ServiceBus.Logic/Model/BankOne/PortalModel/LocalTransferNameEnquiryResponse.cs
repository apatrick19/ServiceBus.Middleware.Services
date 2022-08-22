using ServiceBus.Core.Model.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.PortalModel
{
    public class LocalTransferNameEnquiryResponse:BaseResponse
    {
        public BankOneAccountSummaryModel SourceAccount { get; set; }
        public BankOneAccountSummaryModel DestinationAccount { get; set; }
        public string ReferenceNo { get; set; }
    }
}
