using ServiceBus.Logic.Model.PortalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class BankOneAccountSummaryModel
    {
        public bool IsSuccessful { get; set; }
        public string CustomerIDInString { get; set; }
        public Message Message { get; set; }
        public string TransactionTrackingRef { get; set; }
        public string Page { get; set; }

        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

    }

  
    public class StatementPreference
    {
        public string Delivery { get; set; }
        public string Period { get; set; }
    }

    public class Message
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string NUBAN { get; set; }
        public string AccountNumber { get; set; }
        public string Product { get; set; }
        public string ReferenceNo { get; set; }
        public string AccountOfficer { get; set; }
        public StatementPreference StatementPreference { get; set; }
        public string NotificationPreference { get; set; }
        public string Branch { get; set; }
        public string AccountStatus { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public object IntroducerName { get; set; }
        public string TransactionPermission { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal LedgerBalance { get; set; }
    }


    //public class AccountSummaryBaseResponse:BaseResponse
    //{
    //    public bool IsSuccessful { get; set; }
    //    public string CustomerIDInString { get; set; }
    //    public Message Message { get; set; }
    //    public string TransactionTrackingRef { get; set; }
    //    public string Page { get; set; }
    //}

}
