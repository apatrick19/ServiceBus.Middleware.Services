using ServiceBus.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Model
{
    public class PostingModel:Request
    {
        public string RetrievalReference { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string NibssCode { get; set; }
        public decimal Fee { get; set; }
        public string Narration { get; set; }
        public string Token { get; set; }
        public string GLCode { get; set; }
    }
}
