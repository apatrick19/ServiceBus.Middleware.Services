using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public class TransferModel:Request
    {
        public string SourceAccountNo { get; set; }       
        public string RecievingAccountNo { get; set; }       
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public string PIN { get; set; }
        public bool IsBeneficiaryTransfer { get; set; }

        public string RetrievalReference { get; set; }
    }
}
