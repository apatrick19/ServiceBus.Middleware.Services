using ServiceBus.Core.Contracts;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model
{
    public class Title : Dropdown, ITitle
    {
       public string NavID { get; set; }
    }
}
