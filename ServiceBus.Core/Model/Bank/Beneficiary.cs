using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
  public  class Beneficiary:Entity
    {
        public string InitiatorAccountNo { get; set; }
        public string BeneficiaryBankCode { get; set; }
        public string BeneficiaryBankName { get; set; }
        public string BeneficiaryAccountName { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string SessionId { get; set; }
        public bool isMannyAccount { get; set; }
    }
}
