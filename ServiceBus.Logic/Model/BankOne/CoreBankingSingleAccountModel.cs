namespace ServiceBus.Logic.Model.Validation
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class CoreBankingSingleAccountModel
    {
        [JsonProperty("$id")]
      
        public long Id { get; set; }

        [JsonProperty("Payload")]
        public Payload Payload { get; set; }

        [JsonProperty("ErrorDetails")]
        public object ErrorDetails { get; set; }

        [JsonProperty("ResponseCode")]
        public long ResponseCode { get; set; }
    }

    public partial class Payload
    {
        [JsonProperty("$id")]
      
        public long Id { get; set; }

        [JsonProperty("BranchName")]
        public object BranchName { get; set; }

        [JsonProperty("AccountName")]
        public string AccountName { get; set; }

        [JsonProperty("CustomerName")]
        public string CustomerName { get; set; }

        [JsonProperty("AccountNumber")]
      
        public string AccountNumber { get; set; }

        [JsonProperty("AccountType")]
        public string AccountType { get; set; }

        [JsonProperty("AccountBalance")]
        public double AccountBalance { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("BranchId")]
        public string BranchId { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("BvnNumber")]
        public string BvnNumber { get; set; }

        [JsonProperty("ObjectID")]
        public long ObjectId { get; set; }
    }
}
