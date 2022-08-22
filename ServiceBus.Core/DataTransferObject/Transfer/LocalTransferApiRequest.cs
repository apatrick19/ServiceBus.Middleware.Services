using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.Transfer
{
    public class LocalTransferApiRequest
    {
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public string RetrievalReference { get; set; }
        public string Narration { get; set; }
        public string AuthenticationKey { get; set; }
    }


    //public class OtherBanksTransferNameResponse:Response
    //{
    //    public NameEnquiryResponse Beneficiary { get; set; }      

    //    public BankOneAccountSummaryModel Payer { get; set; }

    //    public string BankName { get; set; }
    //}
}
