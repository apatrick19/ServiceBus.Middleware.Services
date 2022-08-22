using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class CardResponseModel
    {
        public string ResponseMessage { get; set; }
        public string Identifier { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
