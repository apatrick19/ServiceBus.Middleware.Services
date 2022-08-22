using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.Postings
{
   public  class PostingResponse
    {
        public string ResponseCode { get; set; }
        public string Reference { get; set; }
        public bool IsSuccessful { get; set; }
        public string ResponseMessage { get; set; }
    }

  
}
