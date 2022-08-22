using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.AccountInfo
{
    
    public partial class CoreBankingBasicAccountInfoModel
    {
        [JsonProperty("$id")]       
        public long Id { get; set; }

        [JsonProperty("Payload")]
        public Payload Payload { get; set; }

        [JsonProperty("ErrorDetails")]
        public string ErrorDetails { get; set; }

        [JsonProperty("ResponseCode")]
        public long ResponseCode { get; set; }
    }

    public partial class Payload
    {
        [JsonProperty("$id")]
       
        public long Id { get; set; }

        [JsonProperty("CustomerId")]
        public long CustomerId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("IdNumber")]
        public object IdNumber { get; set; }

        [JsonProperty("MotherMaidenName")]
        public string MotherMaidenName { get; set; }
    }
}
