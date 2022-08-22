using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public  class LoanRequestModel
    {
        public string TransactionTrackingRef { get; set; }
        public string LoanProductCode { get; set; }
        public string CustomerID { get; set; }
        public string LinkedAccountNumber { get; set; }
        public int Tenure { get; set; }
        public int Moratorium { get; set; }
        public DateTime InterestAccrualCommencementDate { get; set; }
        public int Amount { get; set; }
        public int PrincipalPaymentFrequency { get; set; }
        public int InterestPaymentFrequency { get; set; }
    }
}
