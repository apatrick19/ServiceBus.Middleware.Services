using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
   public class Complaints:EntityStatus
    {
        public string Description { get; set; }
        public string Title { get; set; }
        public string MobileNo { get; set; }
        public string AccountNo { get; set; }
        public string Resolution { get; set; }
    }
}
