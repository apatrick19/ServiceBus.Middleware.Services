using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface ISMS:IEntityStatus
    {
         string PhoneNumber { get; set; }
         string Message { get; set; }
        
    }
}
