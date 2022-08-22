using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.PortalModel
{
    public class AccountCreationResponse:BaseResponse
    {
        public string AccountNo { get; set; }
        public string AccountType { get; set; }
        public string AccountManager { get; set; }
        public string CustomerID { get; set; }
    }
}
