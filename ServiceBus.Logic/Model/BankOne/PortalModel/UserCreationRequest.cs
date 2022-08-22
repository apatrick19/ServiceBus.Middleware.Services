using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Logic.Model.PortalModel
{
    public class UserCreationRequest
    {
        public int ID { get; set; }
        public string Role { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string MobileNo { get; set; }
        public string Gender { get; set; }
        public string Region { get; set; }
        public string Manager { get; set; }
        public string PassportUrl { get; set; }
    }
}