using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Settings
{
    public class GenericResponseModel
    {
       public string Code { get; set; }
       public string Name { get; set; }
       public string ThirdBind { get; set; }
    }

    public class BranchResponseModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
    }
}
