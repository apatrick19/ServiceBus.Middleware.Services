using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.PortalModel
{
   public  class DropdownResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public double Amount { get; set; }

        public string ParentCode { get; set; }

        public bool isAdmin { get; set; }
    }
}
