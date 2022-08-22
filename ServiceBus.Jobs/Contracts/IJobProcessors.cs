using ServiceBus.Logic.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Jobs.Contracts
{
   public interface IJobProcessors
    {
        ResponseModel GenerateMembershipCertificate();
    }
}
