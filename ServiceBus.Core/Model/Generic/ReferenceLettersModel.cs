using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
   public class ReferenceLettersModel
    {
        public string EmployerName { get; set; }
        public string RSAPin { get; set; }
        public string Title { get; set; }
        public string SurName { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }       
        public string ReferenceName { get; set; }
        public string RefCompanyName { get; set; }
        public string HouseNo { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string UserName { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorSignature { get; set; }
        public string UserSignatureUrl { get; set; }
        public string User { get; set; }
        public bool isReferenceLetter { get; set; }       
        public bool isAccruedRightLetter { get; set; }
        public string Gender { get; set; }

    }
}
