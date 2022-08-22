using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Custom.Model
{
    public class GenericDto<T> where T : class
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        [JsonProperty("@odata.context")]
        public string Context { get; set; }

        //[JsonProperty("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
        //public string Cookie { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public List<T> Value { get; set; }
    }
}
