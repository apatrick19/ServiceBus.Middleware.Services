using ServiceBus.Core.Contracts;
using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
   public class AirtimeBiller:Dropdown, IAirtimeBiller
    {
        public string FileName { get; set; }
        public string ColorCode { get; set; }
    }
}
