using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface IEmail:IEntityStatus
    {
        string Destination { get; set; }
        string Message { get; set; }
    }
}
