using ServiceBus.Core.Contracts.Bank.Interface;
using ServiceBus.Core.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Bank
{
    public class Account:EntityStatus,IAccount
    {
        public string OperatorId { get; set; }
        public string BankId { get; set; }
        public string AccountName { get; set; }
        public string NIN { get; set; }
        public int AccountTier { get; set; }
        public string AcctID { get; set; }
        public string CustomerId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceID { get; set; }
        public string UserName { get; set; }
        public string AccountNumber { get; set; }
        public string BVN { get; set; }
        public string PIN { get; set; }
        public string LastPIN { get; set; }
        public string Password { get; set; }
        public string LastPassword { get; set; }
        public string Country { get; set; }
        public int Gender { get; set; }
        public string Title { get; set; }
        public string MaritalStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Nationality { get; set; }
        public string StateOfOrigin { get; set; }
        public string LGAOfOrigin { get; set; }
        public string ResidentialAddress { get; set; }
        public string ResidentialState { get; set; }
        public string ResidentialLGA { get; set; }
        public string PostalAddress { get; set; }
        public string MotherMaidenName { get; set; }
        public string IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime IdentityExpiryDate { get; set; }
        public string Occupation { get; set; }
        public string OfficeAddress { get; set; }
        public string NOKName { get; set; }
        public string NOKAddress { get; set; }
        public string NOKNo { get; set; }
        public string NOKRelationship { get; set; }
        public string EmployerName { get; set; }
        public string EmployerAddress { get; set; }
        public string EmployerMobile { get; set; }
        public DateTime DateOfArrival { get; set; }
        public string WorkPermitNo { get; set; }
        public string ResidentPermitNo { get; set; }
        public string VisaNo { get; set; }
        public string Passport { get; set; }
        public string Signature { get; set; }
        public string IdentityCard { get; set; }
        public string ProductCode { get; set; }
        public string ReferralName { get; set; }
        public string ReferralPhoneNo { get; set; }
        public string AccountOfficerCode { get; set; }

        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

    }
}
