using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.Quickteller.bills
{

    public class BaseBiller
    {
        public string BillerId { get; set; }
        public string BillerName { get; set; }
    }
    public class BillersResponse
    {
        public string CategoryId { get; set; }
        public int BillerID { get; set; }
        public object BillerCategoryID { get; set; }
        public string Narration { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string CustomerField1 { get; set; }
        public string CustomerField2 { get; set; }
        public string SupportEmail { get; set; }
        public double Surcharge { get; set; }
        public object Url { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public string ShortName { get; set; }
        public string CustomSectionUrl { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public object StatusDetails { get; set; }
        public bool RequestStatus { get; set; }
        public object ResponseDescription { get; set; }
        public object ResponseStatus { get; set; }
    }

  

    

}
