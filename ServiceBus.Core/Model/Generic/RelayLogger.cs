using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class RelayLogger : Entity, IRelayLogger
    {
        public string URL { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime Date { get; set; }
        public int ResponseRate { get; set; }    //ResponseRate measured in seconds (s)

    }
}
