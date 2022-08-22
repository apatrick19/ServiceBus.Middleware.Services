using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class SMSRequestModel
    {
        public string AccountNo { get; set; }
        public string RecipientMobileNo { get; set; }
        public string MessageToSend { get; set; }
        public string TrackingNo { get; set; }
    }
}
