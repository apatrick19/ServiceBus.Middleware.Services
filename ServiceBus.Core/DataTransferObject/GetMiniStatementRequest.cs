using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
   public  class GetMiniStatementRequest : Request
    {        public string AccountNumber { get; set; }
    }
}
