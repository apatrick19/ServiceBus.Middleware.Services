using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class ClientDocument
    {
        public string PassportUpload { get; set; }
        public string SignatureUpload { get; set; }
        public string EvidenceOfEmploymentUpload { get; set; }
        public string ProofOfAddressUpload { get; set; }
        public string ProofOfIdentityUpload { get; set; }
        public string BirthCertificateUpload { get; set; }
    }
}
