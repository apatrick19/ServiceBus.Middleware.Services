using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
   public  class CustomerCardResponseModel
    {
        public bool IsSuccessful { get; set; }

        public string ResponseDescription { get; set; }

        public List<Card> Cards { get; set; }
    }

    public class Card
    {
        public string AccountNumber { get; set; }
        public string CardPAN { get; set; }
        public string SerialNo { get; set; }
        public string LinkedDate { get; set; }
    }

}
