using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Model
{
   public  class LeadModel
    {
        public string AgentCode { get; set; }
        public string FirstName { get; set; }
    
        public string LastName { get; set; }
        
        public string PhoneNumber { get; set; }
      
        public string Email { get; set; }
      
        public string Description { get; set; }    
             
        public string State { get; set; }

        public string Branches { get; set; }

        public string ChannelSource { get; set; }

        public string LeadType { get; set; }

        public string PFA { get; set; }

        public string RSAPin { get; set; }

        public string DocumentUpload { get; set; }
    }
}
