using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.Logic.Model.PortalModel
{
    public class AccountReportRequest
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string ProductType { get; set; }

    }
}