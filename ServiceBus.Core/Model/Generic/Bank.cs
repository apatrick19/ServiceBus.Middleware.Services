using ServiceBus.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Bank : Dropdown, IBank
    {
       public string SortCode { get; set; }
       public string CBNCode { get; set; }
       public string RCNo { get; set; }
    }
}
