using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class SMS:EntityStatus,ISMS
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
