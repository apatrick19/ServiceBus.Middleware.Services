using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public  class BeneficiaryModel:Request
    {
        public string InitiatorAccountNo { get; set; }
        public string BeneficiaryBankCode { get; set; }
        public string BeneficiaryBankName { get; set; }
        public string BeneficiaryAccountName { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public bool isMannyAccount { get; set; }
    }
}
