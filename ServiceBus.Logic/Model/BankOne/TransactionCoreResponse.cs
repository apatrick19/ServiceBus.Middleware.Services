using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.Transactions
{
    public class TransactionCoreResponse
    {
        [JsonProperty("$id")]

        public long Id { get; set; }

        [JsonProperty("Payload")]
        public List<Payload> Payload { get; set; }

        [JsonProperty("ErrorDetails")]
        public object ErrorDetails { get; set; }

        [JsonProperty("ResponseCode")]
        public long ResponseCode { get; set; }
    }

    public class Payload
    {
    
     [JsonProperty("$id")]
    public string id { get; set; }
    public string UserName { get; set; }
    public DateTime TransactionDate { get; set; }
    public string EntryType { get; set; }
    public double Amount { get; set; }
    public double Balance { get; set; }
    public string TransactionType { get; set; }
    public object RefNumber { get; set; }
    public string Comments { get; set; }
    public DateTime ValueDate { get; set; }
    public int ObjectID { get; set; }
}

}
