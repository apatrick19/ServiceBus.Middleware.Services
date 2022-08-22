using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
  public   class AccountEnquiryModel
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Nuban { get; set; }
        public string Number { get; set; }
        public string ProductCode { get; set; }
        public string PhoneNuber { get; set; }
        public string BVN { get; set; }
        public double AvailableBalance { get; set; }
        public double LedgerBalance { get; set; }
        public string Status { get; set; }
        public string Tier { get; set; }
        public double MaximumBalance { get; set; }
        public object MaximumDeposit { get; set; }
        public bool IsSuccessful { get; set; }
        public string ResponseMessage { get; set; }
        public string PNDStatus { get; set; }
        public string LienStatus { get; set; }
        public string FreezeStatus { get; set; }
        public bool RequestStatus { get; set; }
        public string ResponseDescription { get; set; }
        public string ResponseStatus { get; set; }
    }
}
