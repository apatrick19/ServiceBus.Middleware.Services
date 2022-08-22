using ServiceBus.Core.Contracts;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Lga : Dropdown, ILga
    {
        public string StateCode { get; set; }
        public string Code { get; set; }
    }
}
