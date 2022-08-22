using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface IAgentLocator:IEntity
    {
        string Name { get; set; }
        string Code { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
        string Description { get; set; }        
        string JobTitle { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        string AgentType { get; set; }
        string State { get; set; }
        string Region { get; set; }
        string AgentSupervisor { get; set; }
        string RegionalManager { get; set; }


    }
}
