using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class AuditTrail : Entity, IAuditTrail
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public DateTime DateCommitted { get; set; }
        public string Description { get; set; }
        public string Output { get; set; }
        public string CustomerID { get; set; }
    }
}
