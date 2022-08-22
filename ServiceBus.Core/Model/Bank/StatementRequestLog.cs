using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
   public class StatementRequestLog:EntityStatus
    {
        public string AccountNumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Email { get; set; }
        public bool ShareViaEmail { get; set; }
    }
}
