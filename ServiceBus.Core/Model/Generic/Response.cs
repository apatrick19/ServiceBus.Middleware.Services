using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public  class Response : EntityStatus, IResponse
    {
        public virtual string ResponseCode { get; set; }
        public virtual string ResponseDescription { get; set; }
       
    }
}
