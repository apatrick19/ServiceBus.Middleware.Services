using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
   public class Transaction:Entity
    {
        public string Description { get; set; }
        public DateTime TransactionDate{ get; set; }
        public decimal Amount{ get; set; }
        public string TransactionType{ get; set; }
    }
}
