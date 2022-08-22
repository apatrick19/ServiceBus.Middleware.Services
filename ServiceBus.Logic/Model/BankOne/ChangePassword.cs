using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class ChangePassword
    {
        public string username { get; set; }
        public string oldpassword { get; set; }
        public string newpassword { get; set; }
        public string token { get; set; }
    }
}
