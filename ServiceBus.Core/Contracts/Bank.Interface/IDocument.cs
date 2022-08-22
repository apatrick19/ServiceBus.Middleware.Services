using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Bank.Interface
{
    public interface IDocument : IEntityStatus
    {
         string AccountNo { get; set; }
         string Email { get; set; }
         string Passport { get; set; }
         string Signature { get; set; }
         string MeansOfID { get; set; }
    }
}
