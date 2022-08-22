using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class ReversalResponse:Response
    { 
      
        public string Status { get; set; }
        public bool IsSuccessful { get; set; }
      
    }
}
