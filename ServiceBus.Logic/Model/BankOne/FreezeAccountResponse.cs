using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class FreezeAccountResponse: BaseAccountResponse
    {
       
    }

    public class BaseAccountResponse
    {
        public bool RequestStatus { get; set; }
        public string ResponseDescription { get; set; }
        public string ResponseStatus { get; set; }
    }
}
