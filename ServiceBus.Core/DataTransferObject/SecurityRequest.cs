using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
    public class SecurityRequest:Request
    {
        public string AppId { get; set; }
        public string Appkey { get; set; }        
    }
}
