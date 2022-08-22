using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Contracts.Bank.Interface
{
   public interface IAccount:IEntityStatus
    {
        string AccountNumber { get; set; }
        string BVN { get; set; }
        string PIN { get; set; }
        string Password { get; set; }
        string Country { get; set; }
        int Gender { get; set; }
        string Title { get; set; }
        string MaritalStatus { get; set; }
        string FirstName { get; set; }
         string LastName { get; set; }
         string MiddleName { get; set; }         
         DateTime DOB { get; set; }
         string Email { get; set; }
         string MobileNo { get; set; }
         string Nationality { get; set; }
         string StateOfOrigin { get; set; }
         string LGAOfOrigin { get; set; }
         string ResidentialAddress { get; set; }
         string ResidentialState { get; set; }
         string ResidentialLGA { get; set; }
         string PostalAddress { get; set; }
         string MotherMaidenName { get; set; }
         string IdentityType { get; set; }
         string IdentityNumber { get; set; }
         DateTime IdentityExpiryDate { get; set; }
         string Occupation { get; set; }
         string OfficeAddress { get; set; }
         string NOKName { get; set; }
         string NOKAddress { get; set; }
         string NOKNo { get; set; }
         string EmployerName { get; set; }
         string EmployerAddress { get; set; }
         string EmployerMobile { get; set; }
         DateTime DateOfArrival { get; set; }
         string WorkPermitNo { get; set; }
         string ResidentPermitNo { get; set; }
         string VisaNo { get; set; }


    }
}
