using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
  

    public class UserRoleModules : Entity
    {
        public string Name { get; set; }        
        public string Description { get; set; }  
        public bool IsAdmin { get; set; }
        public bool Accounts { get; set; }
        public bool Transfer { get; set; }
        public bool Bills { get; set; }
        public bool Transaction { get; set; }
        public bool Cards { get; set; }
        public bool CRM { get; set; }
        public bool User { get; set; }
        public bool Settings { get; set; }
    }
}
