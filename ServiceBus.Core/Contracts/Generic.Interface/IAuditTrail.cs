using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface IAuditTrail : IEntity
    {
        string Name { get; set; }
        string Action { get; set; }
        string Module { get; set; }
        DateTime DateCommitted { get; set; }
        string Description { get; set; }
        string Output { get; set; }
    }
}
