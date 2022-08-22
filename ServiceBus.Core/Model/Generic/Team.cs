using ServiceBus.Core.Contracts;
using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class Team : Dropdown,ITeam
    {
        public bool AccountTeam { get; set; }
        public bool CaseTeam { get; set; }
        public bool LeadTeam { get; set; }
        public bool BenefitTeam { get; set; }
        public bool DRTeam { get; set; }
        public bool RecordUpdateTeam { get; set; }
    }
}
