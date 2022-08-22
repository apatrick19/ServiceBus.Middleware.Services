using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
    public class GetAccountByAccountNoResponse:Response
    {
        public string AccountName { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal LedgerBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string Status { get; set; }
        public string BranchId { get; set; }
        public string PhoneNumber { get; set; }
        public string BvnNumber { get; set; }
        public string CustomerId { get; set; }
        public string ReferenceNo { get; set; }
        public string StatementPreference { get; set; }
        public string NotificationPreference { get; set; }
        public string Email { get; set; }
        public string TransactionPermission { get; set; }
    }
}
