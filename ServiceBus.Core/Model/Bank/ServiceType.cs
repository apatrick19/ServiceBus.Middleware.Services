using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
   public class ServiceType:EntityStatus
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
