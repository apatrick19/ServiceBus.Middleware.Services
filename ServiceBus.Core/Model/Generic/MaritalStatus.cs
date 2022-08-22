using ServiceBus.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class MaritalStatus: Dropdown, IMaritalStatus
    {
        public string NavCode { get; set; }
        public string PencomCode { get; set; }
    }
}
