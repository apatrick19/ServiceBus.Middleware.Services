using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
   public  interface ITeam:IDropdown
    {
         bool AccountTeam { get; set; }
         bool CaseTeam { get; set; }
         bool LeadTeam { get; set; }
         bool BenefitTeam { get; set; }
         bool DRTeam { get; set; }
         bool RecordUpdateTeam { get; set; }
    }
}
