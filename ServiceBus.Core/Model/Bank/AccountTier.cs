using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
   public  class AccountTier:Entity
    {
        public string Name { get; set; }
        public int Tier { get; set; }
        public decimal MaximumAmount { get; set; }
    }
}
