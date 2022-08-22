using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
    public class AccountCreationResponse:Response
    {
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string AccountManager { get; set; }
        public string CustomerID { get; set; }
    }
}
