using ServiceBus.Core.Contracts.Generic.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Core.Model.Generic
{
    public class User:Entity,IUser
    {
        [StringLength(200), MaxLength(200)]
        public string UserName { get; set; }
        [StringLength(200), MaxLength(200)]
        public string Password { get; set; }
        [StringLength(200), MaxLength(200)]
        public string OldPassword { get; set; }
        // public string Role { get; set; }      
        public int Role { get; set; }
        [StringLength(200), MaxLength(200)]
        public string FirstName { get; set; }
        [StringLength(200), MaxLength(200)]
        public string LastName { get; set; }
        [StringLength(200), MaxLength(200)]
        public string Email { get; set; }
        [StringLength(200), MaxLength(200)]
        public string MobileNo { get; set; }
        public int Gender { get; set; }
        public int Region { get; set; }       
        public string PassportUrl { get; set; }
        public bool IsFirstLogon { get; set; }
        public bool IsPasswordChanged { get; set; }
        public bool IsRegistrationEmailSent { get; set; }
        [StringLength(50), MaxLength(50)]
        public string OperatorId { get; set; }
    }
}
