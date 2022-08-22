using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class ForgotPasswordModel
    {
        public string AccountNumber { get; set; }
     
    }

    public class PasswordUpdateModel
    {
        public string AccountNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }

    }
}
