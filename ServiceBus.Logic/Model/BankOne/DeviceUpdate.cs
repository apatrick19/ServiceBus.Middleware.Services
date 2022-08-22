using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public class DeviceUpdate
    {
        public string AccountNo { get; set; }
        public string MobileNo { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string PIN { get; set; }
    }
}
