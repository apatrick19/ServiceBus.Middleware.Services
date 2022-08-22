using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class BillsPaymentRequest
    {
        public string BillerID { get; set; }
        public string BillerName { get; set; }
        public string BillerCategoryID { get; set; }
        public string BillerCategoryName { get; set; }
        public string PaymentItemID { get; set; }
        public string PaymentItemName { get; set; }
        public string CustomerID { get; set; }
        public string CustomerDepositSlipNumber { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string Token { get; set; }
        public decimal Amount { get; set; }

        public string PIN { get; set; }
    }
}
