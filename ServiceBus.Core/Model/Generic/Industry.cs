using ServiceBus.Core.Contracts;
using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Industry: Dropdown, IIndustry
    {
        public string SectorCode { get; set; }
    }
}
