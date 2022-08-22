using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBus.Logic.Model
{
   public  class BankOneAccountCreationApiRequest
    {
        public string TransactionTrackingRef { get; set; }
        public string CustomerID { get; set; }
        public string AccountOpeningTrackingRef { get; set; }
        public string ProductCode { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public string BVN { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int Gender { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string NationalIdentityNo { get; set; }
        public string NextOfKinPhoneNo { get; set; }
        public string NextOfKinName { get; set; }
        public string ReferralPhoneNo { get; set; }
        public string ReferralName { get; set; }
        public bool HasSufficientInfoOnAccountInfo { get; set; }
        public int AccountInformationSource { get; set; }
        public string OtherAccountInformationSource { get; set; }
        public string AccountOfficerCode { get; set; }
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public string CustomerImage { get; set; }
        public string CustomerSignature { get; set; }
        public int NotificationPreference { get; set; }
        public int TransactionPermission { get; set; }
    }
}
