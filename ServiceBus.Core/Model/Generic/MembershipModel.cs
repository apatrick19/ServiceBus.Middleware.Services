using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Core.Model.Generic
{
    /// <summary>
    /// Membership certificate Model generator
    /// </summary>
    public class MembershipModel
    {
        public string RSAPin { get; set; }
        public string Title { get; set; }
        public string SurName { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public string MobileNo { get; set; }
        public string DOB { get; set; }
        public string EmailAddress { get; set; }
        public string EmployerName{ get; set; }
        public string EmployeeId{ get; set; }
        public bool IsChannelSource{ get; set; }
        public string PassportUrl { get; set; }
        public string Username { get; set; }
    }
}