using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class CustomerCreationResultModel
    {
        [JsonProperty("$id")]
        public string id { get; set; }

        [JsonProperty("Payload")]
        public int Payload { get; set; }

        [JsonProperty("ErrorDetails")]
        public string ErrorDetails { get; set; }

        [JsonProperty("ResponseCode")]
        public int ResponseCode { get; set; }
  
}
}
