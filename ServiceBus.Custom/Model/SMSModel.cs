using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Model
{
    public class SMSModel
    {
        public string MobileNumber { get; set; }
        public string Message { get; set; }
    }

    public class EmailModel
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class ChannelActivitiesModel
    {
        public string RSAPIN { get; set; }
        public string Channel { get; set; }
        public string Activities { get; set; }
        public DateTime ActionDate { get; set; }
    }

    public class ChannelLog
    {
        public string RSAPIN { get; set; }
        public int Channel { get; set; }        
        public DateTime ActionDate { get; set; }
    }
}
