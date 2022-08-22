using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.Quickteller
{
   public  class CategoryResponse
    {
        public string Description { get; set; }
        public bool IsAirtime { get; set; }
        public int BillerCategoryID { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public string StatusDetails { get; set; }
        public string RequestStatus { get; set; }
        public string ResponseDescription { get; set; }
        public string ResponseStatus { get; set; }
    }

   

   
}
