using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Agent : IAgent
    {
        public string UserGuid { get; set; }       
        public string Guid { get; set; }       
        public string Name { get; set; }
        public string Code { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AgentType { get; set; }
        public string State { get; set; }
        public string Region { get; set; }
        public string AgentSupervisor { get; set; }
        public string RegionalManager { get; set; }
    }

    
}
