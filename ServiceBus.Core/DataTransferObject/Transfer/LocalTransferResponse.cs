using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.DataTransferObject
{
  public  class LocalTransferResponse:Response
    {
        public bool IsSuccessful { get; set; }       
        public string Reference { get; set; }
    }
}
