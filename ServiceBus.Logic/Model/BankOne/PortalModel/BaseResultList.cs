using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.PortalModel
{
    public class BaseResultList:ResponseModel
    {
        public List<DropdownResponse> result { get; set; }
    }
}
