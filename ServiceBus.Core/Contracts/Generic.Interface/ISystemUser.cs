using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface ISystemUser
    {
        string Name { get; set; }
        string Email { get; set; }
        string MobileNumber { get; set; }
        string SupervisorName { get; set; }
        string SalesAgent { get; set; }
    }
}
