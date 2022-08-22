using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Generic.Interface
{
    public interface IAccountDocument:IEntityStatus
    {
         string Customerid { get; set; }
         string PassportUpload { get; set; }
         string SignatureUpload { get; set; }
         string ProofOfIdentityUpload { get; set; }
         string ProofOfAddressUpload { get; set; }
         string BirthCertificateUpload { get; set; }
         string EvidenceOfEmploymentUpload { get; set; }         
    }

    public interface IDataRecaptureDocument : IAccountDocument
    {
         string Rsapin { get; set; }
    }
}
