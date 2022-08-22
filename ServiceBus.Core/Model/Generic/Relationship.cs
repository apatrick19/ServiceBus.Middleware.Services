using ServiceBus.Core.Contracts;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Relationship : Dropdown, IRelationship
    {
        public string genderCode { get; set; }
    }
}
