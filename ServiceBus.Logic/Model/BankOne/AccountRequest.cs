using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public class AccountRequest:Request
    {
        public string BVN { get; set; }
      //  public string Nationality { get; set; }
        public string Title { get; set; }
        [Required]
        public int Gender { get; set; }      
        public string MaritalStatus { get; set; }    
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public DateTime DOB { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public string Email { get; set; }
        [Required]
        public string MobileNo { get; set; }
        public string Address { get; set; }
        //public string Passport { get; set; }
       // public string Signature { get; set; }
       // public string IdentityCard { get; set; }
       // public string UserName { get; set; }
       
      //  public string PIN { get; set; }
       
        //public string Password { get; set; }
     //   public string DeviceId { get; set; }
      //  public string DeviceName { get; set; }
      //  public string NOKName { get; set; }
     //   public string NOKPhone { get; set; }
       // public string ProductCode { get; set; }
      //  public string NIN { get; set; }
     //   public string AccountOfficerCode { get; set; }
     //   public string ReferralName { get; set; }
       // public string ReferralPhoneNo { get; set; }


    }
}
