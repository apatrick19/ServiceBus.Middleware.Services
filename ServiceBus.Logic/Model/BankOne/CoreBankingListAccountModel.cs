using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model.CorebankingList
{   

 public class Payload
 {

    [JsonProperty("$id")]
    public string id { get; set; }
    public string CustomerName { get; set; }
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
    public double AccountBalance { get; set; }
    public string Status { get; set; }
    public int BranchId { get; set; }
    public string PhoneNumber { get; set; }
    public string BvnNumber { get; set; }
    public string BranchName { get; set; }
    public int ObjectID { get; set; }

}
   public class CoreBankingListAccountModel
   {
   
      [JsonProperty("$id")]
      public string id { get; set; }
      public List<Payload> Payload { get; set; }
      public string ErrorDetails { get; set; }
      public int ResponseCode { get; set; }
   
   }
}
