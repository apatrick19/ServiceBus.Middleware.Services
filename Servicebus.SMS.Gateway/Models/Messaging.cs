using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Servicebus.SMS.Gateway.Models
{
    public class Messaging
    {
        public long Id { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }
        public int RetrialCount { get; set; }
        public string ThirdPartyResponse { get; set; }
        public string SystemResponse { get; set; }       

        public DateTime? TransactionDate { get; set; } = DateTime.Now;

        public Messaging()
        {
            this.TransactionDate = DateTime.UtcNow;
        }
    }
}